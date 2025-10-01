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
    private InputActions inputActions;
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
        fighting = false;
    }

    public void Initialize(int fn, FencerType ft)
    {
        // set instance variables
        fencerId = fn;
        fencerType = ft;

        if (fencerType == FencerType.Player)
        {
            inputActions = new InputActions();
            movement = fencerId == F0 ? inputActions.Player.P0Movement : inputActions.Player.P1Movement;
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
}
