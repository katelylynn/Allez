using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerStateAudioController : MonoBehaviour
{

    public enum MovementState { Idle, Walk, StepForward, StepBackward, Lunge, LungeCenter, Backdash }
    public enum FoilState { Idle, Attack, ParryLeft, Parried }

    public MovementState movementState = MovementState.Idle;
    public FoilState foilState = FoilState.Idle;

    private Animator animator;
    private int movementLayer;
    private int foilLayer;

    [Header("Sound Clips")]
    public AudioClip Jump;
    public AudioClip Hit;
    public AudioClip Step;
    public AudioClip Lunge;
    public AudioClip Parry;
    public AudioClip Swing;

    private AudioSource source;

    void Awake()
    {
        // Animator setup
        animator = GetComponent<Animator>();
        movementLayer = animator.GetLayerIndex("Movement Layer");
        foilLayer = animator.GetLayerIndex("Foil Layer");

        if (movementLayer < 0) Debug.LogError("Animator layer not found: Movement Layer");
        if (foilLayer < 0) Debug.LogError("Animator layer not found: Foil Layer");

        // Audio setup
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.dopplerLevel = 0f;
    }

    void Update()
    {
        UpdateMovementState();
        UpdateFoilState();
    }

    // ------------------------------
    // MOVEMENT STATE
    // ------------------------------
    void UpdateMovementState()
    {
        if (movementLayer < 0) return;

        var info = animator.GetCurrentAnimatorStateInfo(movementLayer);
        var newState = movementState;

        if (info.IsName("Idle")) newState = MovementState.Idle;
        else if (info.IsName("Walk")) newState = MovementState.Walk;
        else if (info.IsName("StepForward")) newState = MovementState.StepForward;
        else if (info.IsName("StepBackward")) newState = MovementState.StepBackward;
        else if (info.IsName("Lunge")) newState = MovementState.Lunge;
        else if (info.IsName("Lunge Center")) newState = MovementState.LungeCenter;
        else if (info.IsName("Backdash")) newState = MovementState.Backdash;

        if (newState != movementState)
        {
            Debug.Log($"[Movement Layer] {movementState} ➜ {newState}");
            movementState = newState;

            // Play sound only on change
            switch (newState)
            {
                case MovementState.Walk:
                case MovementState.StepForward:
                case MovementState.StepBackward:
                case MovementState.Backdash:
                    Play(Step);
                    break;
                case MovementState.Lunge:
                case MovementState.LungeCenter:
                    Play(Lunge);
                    break;
            }
        }
    }

    // ------------------------------
    // FOIL (COMBAT) STATE
    // ------------------------------
    void UpdateFoilState()
    {
        if (foilLayer < 0) return;

        var info = animator.GetCurrentAnimatorStateInfo(foilLayer);
        var newState = foilState;

        if (info.IsName("Idle")) newState = FoilState.Idle;
        else if (info.IsName("Attack")) newState = FoilState.Attack;
        else if (info.IsName("ParryLeft")) newState = FoilState.ParryLeft;
        else if (info.IsName("Parried")) newState = FoilState.Parried;

        if (newState != foilState)
        {
            Debug.Log($"[Foil Layer] {foilState} ➜ {newState}");
            foilState = newState;

            // Play sound only on change
            switch (newState)
            {
                case FoilState.Attack:
                case FoilState.ParryLeft:
                    Play(Swing);
                    break;
                case FoilState.Parried:
                    Play(Parry);
                    break;
            }
        }
    }

    // ------------------------------
    // AUDIO HELPER
    // ------------------------------
    private void Play(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
            source.PlayOneShot(clip, volume);
    }
}
