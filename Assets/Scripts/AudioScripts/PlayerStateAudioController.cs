using UnityEngine;

public class PlayerStateAudioController : MonoBehaviour
{
    public enum MovementState
    {
        Idle,
        Walk,
        StepForward,
        StepBackward,
        Lunge,
        LungeCenter,
        Backdash
    }
    
    public enum FoilState
    {
        Idle,
        Attack,
        ParryLeft,
        Parried
    }

    public MovementState movementState = MovementState.Idle;
    public FoilState foilState = FoilState.Idle;

    private MovementState lastMovementState;
    private FoilState lastFoilState;

    
    private Animator animator;
    private AudioSource audioSource;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovementState();
        UpdateFoilState();
    }

    void UpdateMovementState()
    {
        // Example check using Animator
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Movement Layer"));
        MovementState newState = movementState;

        if (info.IsName("Idle")) movementState = MovementState.Idle;
        else if (info.IsName("Walk")) movementState = MovementState.Walk;
        else if (info.IsName("StepForward")) movementState = MovementState.StepForward;
        else if (info.IsName("StepBackward")) movementState = MovementState.StepBackward;
        else if (info.IsName("Lunge")) movementState = MovementState.Lunge;
        else if (info.IsName("Lunge Center")) movementState = MovementState.LungeCenter;
        else if (info.IsName("Backdash")) movementState = MovementState.Backdash;
        
        // Log when state changes
        if (newState != movementState)
        {
            Debug.Log($"[Movement Layer] State changed: {movementState} ➜ {newState}");
            movementState = newState;
        }
    }
    
        void UpdateFoilState()
    {
        // Second layer (combat/foil)
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Foil Layer"));
        MovementState newState = movementState;

        if (info.IsName("Idle")) foilState = FoilState.Idle;
        else if (info.IsName("Attack")) foilState = FoilState.Attack;
        else if (info.IsName("ParryLeft")) foilState = FoilState.ParryLeft;
        else if (info.IsName("Parried")) foilState = FoilState.Parried;

        // Log when state changes
        if (newState != movementState)
        {
            Debug.Log($"[Foil Layer] State changed: {movementState} ➜ {newState}");
            movementState = newState;
        }
    }
}
