using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class MovementSound : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAxis;
    public InputActionReference tiltAxis;

    [Header("Step Sound")]
    public AudioClip moveStep;      
    public float stepInterval = 0.3f;

    [Header("Tilt Sound")]
    public AudioClip tiltSound;
    public float tiltInterval = 0.2f;

    private AudioSource source;
    private float lastStepTime;
    private float lastTiltTime;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
    }

    private void OnEnable()
    {
        if (moveAxis != null) moveAxis.action.Enable();
        if (tiltAxis != null) tiltAxis.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAxis != null) moveAxis.action.Disable();
        if (tiltAxis != null) tiltAxis.action.Disable();
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
                source.PlayOneShot(moveStep);

            lastStepTime = Time.time;
        }

        // Tilts
        if (tiltAxis != null)
        {
            float tiltValue = tiltAxis.action.ReadValue<float>();

            if (Mathf.Abs(tiltValue) > 0.1f &&
                Time.time - lastTiltTime > tiltInterval)
            {
                if (tiltSound != null)
                    source.PlayOneShot(tiltSound);

                lastTiltTime = Time.time;
            }
        }
    }

}
