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

    public void Initialize(int fn, FencerType ft)
    {
        // set instance variables
        fencerId = fn;
        fencerType = ft;

        // setup player input
        SetupPlayerInput();

        // set camera position
        cam = GetComponentInChildren<Camera>(); 
        Rect r = cam.rect;
        r.x = (fencerId == F0 ? 0f : 0.5f);
        cam.rect = r;

        // set fencer position, deactivating to overcome rigidbody
        ResetFencer();

        // set event callbacks
        EventManager.RoundStart += () => {
            playerInput.enabled = true;
        };
        EventManager.RoundEnd += ResetFencer;
    }

    private void SetupPlayerInput()
    {
        playerInput = GetComponent<PlayerInput>();

        if (fencerType == FencerType.Player)
        {
            if (fencerId == 0)
                playerInput.actions = p0ActionAsset;
            else if (fencerId == 1)
                playerInput.actions = p1ActionAsset;

            playerInput.defaultActionMap = "Player";
        }
        else if (fencerType == FencerType.AI)
        {
            playerInput.enabled = false;
        }
    }

    private void ResetFencer(int winner = -1)
    {
        playerInput.enabled = false;

        gameObject.SetActive(false);
        gameObject.transform.position = startingPos[fencerId];
        gameObject.transform.rotation = startingRot[fencerId];
        gameObject.SetActive(true);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Fencer"))
        {
            EventManager.TriggerRoundEnd(fencerId);
        }
    }

    public void Update()
    {
        if (fencerType == FencerType.AI)
            CalculateNextMove();
    }

    private void CalculateNextMove()
    {
        Debug.Log("calculating next move");
    }
}
