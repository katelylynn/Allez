/*
    Fencer Audio Controller
    Controls the sound effects for a fencer's actions.
*/

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class FencerAudioController : MonoBehaviour
{
    [Header("Sound Clips")]
    public AudioClip Hit;
    public AudioClip Lunge;
    public AudioClip Parry;
    public AudioClip Swing;
    public AudioClip BackDash;
    public AudioClip Ouch;

    [Header("Female Sound Clips")]
    public AudioClip LungeFemale;
    public AudioClip OuchFemale;
    public AudioClip BackDashFemale;

    [Header("Male Sound Clips")]
    public AudioClip LungeMale;
    public AudioClip OuchMale;
    public AudioClip BackDashMale;

    [Header("Input")]
    public InputActionReference moveAxis;
    public InputActionReference move2Axis;

    [Header("Step Sound")]
    public AudioClip moveStep;
    public float stepInterval = 0.3f;

    [Header("move2 Sound")]
    public AudioClip move2Sound;
    public float move2Interval = 0.3f;

    private AudioSource source;
    private float lastStepTime;
    private float lastmove2Time;

    public void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
    }

    private void OnEnable()
    {
        if (moveAxis != null) moveAxis.action.Enable();
        if (move2Axis != null) move2Axis.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAxis != null) moveAxis.action.Disable();
        if (move2Axis != null) move2Axis.action.Disable();
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        // Block sounds during countdown / round transitions / outside gameplay
        if (!GameManager.Instance.isGameActive || GameManager.Instance.IsRoundBusy)
            return;

        // Steps (moveAxis)
        if (moveAxis == null) return;

        float value = moveAxis.action.ReadValue<float>();

        if (Mathf.Abs(value) > 0.1f && Time.time - lastStepTime > stepInterval)
        {
            if (moveStep != null)
                source.PlayOneShot(moveStep, 0.4f);

            lastStepTime = Time.time;
        }

        // move2s
        if (move2Axis != null)
        {
            float move2Value = move2Axis.action.ReadValue<float>();

            if (Mathf.Abs(move2Value) > 0.1f && Time.time - lastmove2Time > move2Interval)
            {
                if (move2Sound != null)
                    source.PlayOneShot(move2Sound, 0.4f);

                lastmove2Time = Time.time;
            }
        }
    }

    public void SetGenderAudios(FencerId id)
    {
        // Female
        if (id == FencerId.Fencer0)
        {
            Lunge = LungeFemale;
            Ouch = OuchFemale;
            BackDash = BackDashFemale;
        }
        // Male
        else if (id == FencerId.Fencer1)
        {
            Lunge = LungeMale;
            Ouch = OuchMale;
            BackDash = BackDashMale;
        }
    }

    public void PlayAttack() => Play(Hit);
    public void PlayLunge() => Play(Lunge);
    public void PlayParry() => Play(Parry);
    public void PlaySwing() => Play(Swing);
    public void PlayBackDash() => Play(BackDash);
    public void PlayOuch() => Play(Ouch);

    private void Play(AudioClip clip, float volume = 1f)
    {
        if (clip != null) source.PlayOneShot(clip, volume);
    }
}
