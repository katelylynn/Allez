using UnityEngine;
using UnityEngine.InputSystem;

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
    public Animator animator;

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

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Initialize(FencerId fn, FencerType ft)
    {
        // set instance variables
        fencerId = fn;
        fencerType = ft;

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
            if (fencerId == FencerId.Fencer0)
                playerInput.actions = p0ActionAsset;
            else if (fencerId == FencerId.Fencer1)
                playerInput.actions = p1ActionAsset;

            playerInput.defaultActionMap = "Player";
        }
        else if (fencerType == FencerType.AI)
        {
            playerInput.enabled = false;
        }
    }

    public AnimatorStateInfo GetStateSnapshot(int layer)
    {
        return animator.GetCurrentAnimatorStateInfo(layer);
    }

    private void ResetFencer(FencerId winner = FencerId.None)
    {
        playerInput.enabled = false;

        gameObject.SetActive(false);
        gameObject.transform.position = startingPos[(int)fencerId];
        gameObject.transform.rotation = startingRot[(int)fencerId];
        gameObject.SetActive(true);
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
