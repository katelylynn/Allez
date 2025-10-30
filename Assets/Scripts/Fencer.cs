using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering.Universal;

public enum FencerType
{
    Player,
    AI
}

public enum FencerId : int
{
    None,
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

    // input variables
    private PlayerInput playerInput;
    public InputActionAsset p0ActionAsset;
    public InputActionAsset p1ActionAsset;

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

    public void Update()
    {
        if (fencerType == FencerType.AI)
            CalculateNextMove();
    }

    private void OnRoundStart()
    {
        if (playerInput != null)
            playerInput.enabled = true;
    }

    private void OnDestroy()
    {
        EventManager.RoundEnd -= ResetFencer;
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

        // set fencer position, deactivating to overcome rigidbody
        ResetFencer();

        // set event callbacks
        EventManager.RoundStart += OnRoundStart;
        EventManager.RoundEnd += ResetFencer;
    }
    
    private void SetupPlayerInput()
    {
        playerInput = GetComponent<PlayerInput>();

        if (fencerType == FencerType.Player)
        {
            playerInput.actions = fencerId == FencerId.Fencer0 ? p0ActionAsset : p1ActionAsset;
            playerInput.defaultActionMap = "Player";
        }
        else if (fencerType == FencerType.AI)
        {
            playerInput.enabled = false;
        }
    }

    public void SetAimTarget(Transform target) {
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
        } else {
            Debug.Log("Rigbuilder is null!");
        }
    }

    private void ResetFencer(FencerId winner = FencerId.None)
    {
        playerInput.enabled = false;
        gameObject.GetComponent<Mover>().SetForwardMovement(true);
        gameObject.SetActive(false);
        gameObject.transform.position = startingPos[(int)fencerId];
        gameObject.transform.rotation = startingRot[(int)fencerId];
        gameObject.SetActive(true);
    }

    public AnimatorStateInfo GetStateSnapshot(int layer)
    {
        return anim.GetCurrentAnimatorStateInfo(layer);
    }

    private void CalculateNextMove()
    {
        Debug.Log("calculating next move");
    }
}
