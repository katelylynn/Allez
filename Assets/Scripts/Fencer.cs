using UnityEngine;
using UnityEngine.InputSystem;

public enum FencerType
{
    Player,
    AI
}

public class Fencer : MonoBehaviour
{
    // static variables
    private const int F0 = 0;
    private const int F1 = 1;

    // instance variables
    private int fencerId;
    private FencerType fencerType;
    public bool fighting;

    // input variables
    private P0InputActions p0InputActions;
    private P1InputActions p1InputActions;
    private InputAction movementInput;
    private InputAction lungeInput;
    private InputAction backdashInput;
    private InputAction attackInput;
    private InputAction tiltInput;

    // scene variables
    private Rigidbody rb;
    private Camera cam;
    private Vector3[] startingPos = {
        new Vector3(0, 0, -5),
        new Vector3(0, 0, 5)
    };
    private Quaternion[] startingRot = {
        Quaternion.Euler(0f, 0f, 0f),
        Quaternion.Euler(0f, 180f, 0f)
    };

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(int fn, FencerType ft)
    {
        // set instance variables
        fencerId = fn;
        fencerType = ft;

        if (fencerType == FencerType.Player)
            SetupInputActions();

        // set camera position
        cam = GetComponentInChildren<Camera>(); 
        Rect r = cam.rect;
        r.x = (fencerId == F0 ? 0f : 0.5f);
        cam.rect = r;

        // set fencer position, deactivating to overcome rigidbody
        ResetPosition();

        // set event callbacks
        fighting = false;
        EventManager.RoundStart += () => {
            fighting = true; 
        };
        EventManager.RoundEnd += OnRoundEnd;
    }

    private void SetupInputActions()
    {
        if (fencerId == F0)
        {
            p0InputActions = new P0InputActions();
            movementInput = p0InputActions.Player.Movement;
            lungeInput = p0InputActions.Player.Lunge;
            backdashInput = p0InputActions.Player.Backdash;
            tiltInput = p0InputActions.Player.Tilt;
            attackInput = p0InputActions.Player.Attack;
        }
        else if (fencerId == F1)
        {
            p1InputActions = new P1InputActions();
            movementInput = p1InputActions.Player.Movement;
            lungeInput = p1InputActions.Player.Lunge;
            backdashInput = p1InputActions.Player.Backdash;
            tiltInput = p1InputActions.Player.Tilt;
            attackInput = p1InputActions.Player.Attack;
        }

        movementInput.Enable();
        lungeInput.Enable();
        backdashInput.Enable();
        tiltInput.Enable();
        attackInput.Enable();
    }

    private void ResetPosition()
    {
        gameObject.SetActive(false);
        gameObject.transform.position = startingPos[fencerId];
        gameObject.transform.rotation = startingRot[fencerId];
        gameObject.SetActive(true);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fencer"))
        {
            // NOT IMPLEMENTED
            // On sword collision w torso, trigger round end
        }
    }

    public void OnRoundEnd(int winner)
    {
        fighting = false;
        ResetPosition();
    }

    public void Update()
    {
        if (fighting && fencerType == FencerType.Player)
            ReceiveInput();

        else if (fighting && fencerType == FencerType.AI)
            CalculateNextMove();
    }

    // Keyboard or controller input
    private void ReceiveInput()
    {
        Move(movementInput.ReadValue<float>());

        if (lungeInput.WasPerformedThisFrame())
            Lunge();
        
        if (backdashInput.WasPerformedThisFrame())
            Backdash();

        if (attackInput.WasPressedThisFrame())
            Attack();

        Tilt(tiltInput.ReadValue<float>());
    }

    // AI decision making
    private void CalculateNextMove()
    {
        Debug.Log("calculating next move");
    }

    private void Move(float amount)
    {
        rb.AddForce(new Vector3(0f, 0f, amount), ForceMode.VelocityChange);
    }

    private void Lunge()
    {
        Debug.Log("Lunge");
    }

    private void Backdash()
    {
        Debug.Log("Backdash");

    }

    private void Tilt(float direction)
    {
        if (direction == -1) Debug.Log("tilting left");
        if (direction == 0) Debug.Log("neutral");
        if (direction == 1) Debug.Log("tilting right");
    }

    private void Attack()
    {
        Debug.Log("fencer" + fencerId + " attacking!");

        // TEMP while collision enter not implemented
        EventManager.TriggerRoundEnd(fencerId);
    }

    // cleanup
    private void OnDestroy()
    {
        p0InputActions?.Dispose();
        p1InputActions?.Dispose();
    }
}
