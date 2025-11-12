using UnityEngine;

public class PlayerStateAudioController : MonoBehaviour
{
    public enum MovementState { Idle, Walk, StepForward, StepBackward, Lunge, LungeCenter, Backdash }
    public enum FoilState     { Idle, Attack, ParryLeft, Parried }

    public MovementState movementState = MovementState.Idle;
    public FoilState foilState = FoilState.Idle;

    Animator animator;
    int movementLayer;
    int foilLayer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        movementLayer = animator.GetLayerIndex("Movement Layer");
        foilLayer     = animator.GetLayerIndex("Foil Layer");

        if (movementLayer < 0) Debug.LogError("Animator layer not found: Movement Layer");
        if (foilLayer < 0)     Debug.LogError("Animator layer not found: Foil Layer");
    }

    void Update()
    {
        UpdateMovementState();
        UpdateFoilState();
    }

    void UpdateMovementState()
    {
        if (movementLayer < 0) return;

        var info = animator.GetCurrentAnimatorStateInfo(movementLayer);
        var newState = movementState; // start with current, then detect

        if (info.IsName("Idle"))           newState = MovementState.Idle;
        else if (info.IsName("Walk"))      newState = MovementState.Walk;
        else if (info.IsName("StepForward"))  newState = MovementState.StepForward;
        else if (info.IsName("StepBackward")) newState = MovementState.StepBackward;
        else if (info.IsName("Lunge"))        newState = MovementState.Lunge;
        else if (info.IsName("Lunge Center")) newState = MovementState.LungeCenter;
        else if (info.IsName("Backdash"))     newState = MovementState.Backdash;

        if (newState != movementState)
        {
            Debug.Log($"[Movement Layer] {movementState} ➜ {newState}");
            movementState = newState;
        }
    }

    void UpdateFoilState()
    {
        if (foilLayer < 0) return;

        var info = animator.GetCurrentAnimatorStateInfo(foilLayer);
        var newState = foilState;

        if (info.IsName("Idle"))           newState = FoilState.Idle;
        else if (info.IsName("Attack"))    newState = FoilState.Attack;
        else if (info.IsName("ParryLeft")) newState = FoilState.ParryLeft;
        else if (info.IsName("Parried"))   newState = FoilState.Parried;

        if (newState != foilState)
        {
            Debug.Log($"[Foil Layer] {foilState} ➜ {newState}");
            foilState = newState;
        }
    }
}
