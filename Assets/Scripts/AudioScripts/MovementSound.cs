using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class MovementSound : MonoBehaviour
{
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

    private void Awake()
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
        // Debug.Log($"input value {value}");

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

}