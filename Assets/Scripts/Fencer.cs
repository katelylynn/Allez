using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public enum FencerType
{
    Player,
    AI
}

public enum FencerId : int
{
    None = -1,
    Fencer0 = 0,
    Fencer1 = 1,
}

public class Fencer : MonoBehaviour
{
    // instance variables
    public FencerId fencerId;
    private FencerType fencerType;
    public Animator anim;
    public Transform aimTarget;
    public GameObject foilHitbox;

    // input variables
    private PlayerInput playerInput;
    public InputActionAsset p0ActionAsset;
    //public InputActionAsset p1ActionAsset;
    private string kbGroup;
    private Gamepad myPad;

    // scene variables
    private Camera cam;
    private Vector3[] startingPos = {
        new Vector3(0, 0, -5),
        new Vector3(0, 0, 5)
    };
    private Quaternion[] startingRot = {
        Quaternion.Euler(0f, 0f, 0f),
        Quaternion.Euler(0f, 180f, 0f)
    };

    // scripts
    public Mover mover;
    public Fighter fighter;

    private bool initialized = false;

    private void OnEnable()
    {
        InputSystem.onDeviceChange += HandleDeviceChange; //allows for hot swapping gamepads
    }

    private void OnDisable()
    {
        if (playerInput != null) playerInput.onControlsChanged -= OnControlsChanged;
        InputSystem.onDeviceChange -= HandleDeviceChange;
    }

    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!(device is Gamepad)) return;

        // if a pad event (add/remove/reconnect) occurs, re-evaluate our assignment + filters
        switch (change)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
            case InputDeviceChange.Reconnected:
            case InputDeviceChange.Enabled:
            case InputDeviceChange.Disabled:
                {
                    var newPad = PickPadForFencer(fencerId);
                    if (newPad != myPad)
                    {
                        myPad = newPad;
                        SetActionsDeviceFilter(); 
                        ApplyMask();                                         
                    }
                    break;
                }
            default: break;
        }
    }
    
    private void OnRoundStart()
    {
        if (playerInput != null)
            playerInput.enabled = true;
    }

    private void OnDestroy()
    {
        EventManager.RoundStart -= OnRoundStart;
        EventManager.RoundReset -= OnRoundReset;
        EventManager.InputEnable -= OnInputEnable;
        EventManager.Pause -= OnPauseToggled;
    }

    public void Initialize(FencerId fn, FencerType ft)
    {
        // set instance variables
        fencerId = fn;
        fencerType = ft;
        anim = GetComponent<Animator>();

        // setup player input
        SetupPlayerInput();

        // set camera position
        cam = GetComponentInChildren<Camera>();
        Rect r = cam.rect;
        r.x = (fencerId == FencerId.Fencer0 ? 0f : 0.5f);
        cam.rect = r;

        OnRoundReset();
        // set event callbacks
        EventManager.RoundStart += OnRoundStart;
        EventManager.RoundReset += OnRoundReset;
        EventManager.InputEnable += OnInputEnable;
        EventManager.Pause += OnPauseToggled;

    }

    private void SetupPlayerInput()
    {
        playerInput = GetComponent<PlayerInput>();

        if (fencerType != FencerType.Player) { playerInput.enabled = false; return; }

        playerInput.actions = p0ActionAsset;
        playerInput.defaultActionMap = "Player";
        playerInput.neverAutoSwitchControlSchemes = true;
        playerInput.defaultControlScheme = "";                    // don't let control schemes overwrite bindingMask
        playerInput.notificationBehavior = PlayerNotifications.SendMessages; // so it works with player input behavior

        kbGroup = (fencerId == FencerId.Fencer0) ? "KeyboardP1" : "KeyboardP2";

        PairDevicesOptional();

        ApplyMask();

        playerInput.actions.Enable();
        playerInput.SwitchCurrentActionMap("Player");

        // subscribe to changes late to avoid timing issues
        initialized = true;
        playerInput.onControlsChanged -= OnControlsChanged;
        playerInput.onControlsChanged += OnControlsChanged;

        //Debug.Log($"[{name}] Map: {playerInput.currentActionMap?.name}, Enabled: {playerInput.currentActionMap?.enabled}");
        //Debug.Log($"[{name}] Mask: {playerInput.actions.bindingMask}");
    }

    //
    private void PairDevicesOptional()
    {
        myPad = PickPadForFencer(fencerId);
        SetActionsDeviceFilter();
        //Debug.Log($"[{name}] No gamepad available? {(myPad == null ? "Yes" : "No")}");
        //Debug.Log($"[{name}] Filtered devices: {string.Join(", ", playerInput.actions.devices)}");
    }

    private void SetActionsDeviceFilter()
    {
        
        if (myPad == null) //allows for the case where 0 pads are plugged in
        {
            playerInput.actions.devices = default; // clears filter => all devices
        }
        else
        {
            // if there is a pad, restrict access to pad and keyboard 
            var list = new System.Collections.Generic.List<InputDevice>();
            if (Keyboard.current != null) list.Add(Keyboard.current);
            list.Add(myPad);
            playerInput.actions.devices = new ReadOnlyArray<InputDevice>(list.ToArray());
        }

        InputActionMap map = playerInput.currentActionMap ?? playerInput.actions.FindActionMap("Player", true);
        if (map.enabled) { map.Disable(); map.Enable(); } // this forces bindings to reset
    }
   
    //for hot swapping controllers
    private void OnControlsChanged(PlayerInput pi)
    {
        if (!initialized) return;

        Gamepad newPad = PickPadForFencer(fencerId);
        myPad = newPad; 

        SetActionsDeviceFilter();
        ApplyMask();
    }

    // sets mask on players input so only their respective keyboard controls and gamepad (if present) are used
    private void ApplyMask()
    {
        if (playerInput == null) return;
        var actions = playerInput.actions;
        if (actions == null) return;
        if (string.IsNullOrEmpty(kbGroup)) return;

        // if this fencer has no pad yet, don't include Gamepad group
        if (myPad == null)
            actions.bindingMask = InputBinding.MaskByGroups(kbGroup);
        else
            actions.bindingMask = InputBinding.MaskByGroups(kbGroup, "Gamepad");
    }

    //gets gamepad device for fencer based on their fencerid
    private static Gamepad PickPadForFencer(FencerId id)
    {
        var ordered = Gamepad.all.OrderBy(g => g.deviceId).ToList();
        int idx = (int)id;
        if (idx < 0 || idx >= ordered.Count) return null;
        return ordered[idx];
    }

    public void SetAimTarget(Transform target)
    {
        var headAim = transform.Find("Rig 1/HeadAimRig");
        var foilAim = transform.Find("Rig 1/FoilAimRig");

        MultiAimConstraint headAimConstraint = headAim.GetComponent<MultiAimConstraint>();
        MultiAimConstraint foilAimConstraint = foilAim.GetComponent<MultiAimConstraint>();

        // Head
        var headData = headAimConstraint.data;
        var headSources = headData.sourceObjects;
        headSources.SetTransform(0, target);
        headData.sourceObjects = headSources;
        headAimConstraint.data = headData; // reassign to apply

        // Foil
        var foilData = foilAimConstraint.data;
        var foilSources = foilData.sourceObjects;
        foilSources.SetTransform(0, target);
        foilData.sourceObjects = foilSources;
        foilAimConstraint.data = foilData;

        // Rebuild RigBuilder
        RigBuilder rigBuilder = headAimConstraint.GetComponentInParent<RigBuilder>();
        if (rigBuilder != null)
        {
            rigBuilder.Build();
        }
        else
        {
            Debug.Log("Rigbuilder is null!");
        }
    }

    private void OnRoundReset()
    {
        gameObject.GetComponent<ScriptedMotionPlayer>().StopCurrentMotion();
        gameObject.GetComponent<Mover>().SetForwardMovement(true);
        ToggleComponentsAndChildren(gameObject, false);
        gameObject.transform.position = startingPos[(int)fencerId];
        gameObject.transform.rotation = startingRot[(int)fencerId];
        ToggleComponentsAndChildren(gameObject, true);
        gameObject.GetComponent<Mover>().ZeroVelocity();
        gameObject.GetComponent<Fighter>().ResetSword();
        if (fencerType == FencerType.AI)
            gameObject.GetComponent<AI>().OnRoundReset();
        //foilHitbox.GetComponentInChildren<MeshRenderer>().enabled = false;
    }

    public AnimatorStateInfo GetStateSnapshot(int layer)
    {
        return anim.GetCurrentAnimatorStateInfo(layer);
    }

    private void OnInputEnable(bool enabled)
    {
        if (playerInput) playerInput.enabled = enabled && (fencerType == FencerType.Player);
        if (mover) mover.enabled = enabled;
        if (fighter) fighter.enabled = enabled;
    }

    private void OnPause()
    {
        EventManager.TriggerPause();
    }

    private void ToggleComponentsAndChildren(GameObject go, bool toggle)
    {
        // Disable all components except Transform
        foreach (Component comp in go.GetComponents<Component>())
        {
            if (comp is Transform || comp is AI) continue;

            if ((comp is Mover || comp is Fighter || comp is PlayerInput) && toggle == true) continue;

            if (comp is Behaviour b)
                b.enabled = toggle;

            if (comp is Renderer r)
                r.enabled = toggle;

            if (comp is Collider c)
                c.enabled = toggle;

            if (comp is Rigidbody rb)
                rb.isKinematic = !toggle;
        }
        // Ensures volume stays on bc toggling it on/off breaks its functionality
        foreach (Transform child in go.transform)
        {
            if (child.name == "LowStaminaVolume") continue;
            child.gameObject.SetActive(toggle);
        }
    }

    private void OnPauseToggled()
    {
        playerInput.enabled = !playerInput.enabled;
    }
}
