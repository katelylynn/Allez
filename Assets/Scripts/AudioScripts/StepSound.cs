using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class MovementSound : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAxis;

    [Header("Sound")]
    public AudioClip moveStep;      
    public float stepInterval = 0.3f;

    private AudioSource source;
    private float lastStepTime;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
    }

    private void OnEnable()
    {
        if (moveAxis != null)
            moveAxis.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAxis != null)
            moveAxis.action.Disable();
    }

    private void Update()
    {
        if (moveAxis == null) return;

        float value = moveAxis.action.ReadValue<float>();

        if (Mathf.Abs(value) > 0.1f && Time.time - lastStepTime > stepInterval)
        {
            if (moveStep != null)
                source.PlayOneShot(moveStep);

            lastStepTime = Time.time;
        }
    }
}
