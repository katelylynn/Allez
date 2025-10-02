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

    // player only variables
    private P0InputActions p0InputActions;
    private P1InputActions p1InputActions;
    private InputAction movement;
    private Vector2 input;

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
        {
            if (fencerId == F0)
            {
                p0InputActions = new P0InputActions();
                movement = p0InputActions.Player.Movement;
            }
            else if (fencerId == F1)
            {
                p1InputActions = new P1InputActions();
                movement = p1InputActions.Player.Movement;
            }

            movement.Enable();
        }

        // set camera position
        cam = GetComponentInChildren<Camera>(); 
        Rect r = cam.rect;
        r.x = (fencerId == F0 ? 0f : 0.5f);
        cam.rect = r;

        // set fencer position, deactivating to overcome rigidbody
        gameObject.SetActive(false);
        gameObject.transform.position = startingPos[fencerId];
        gameObject.transform.rotation = startingRot[fencerId];
        gameObject.SetActive(true);

        // set event callbacks
        fighting = false;
        EventManager.RoundStart += () => {
            fighting = true; 
        };
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
        Move(movement.ReadValue<Vector2>().y);
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

    private void OnDestroy()
    {
        p0InputActions?.Dispose();
        p1InputActions?.Dispose();
    }
}
