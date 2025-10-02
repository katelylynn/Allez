using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{

    [Header("Player Tilt Settings")]
    private float tiltDirection;
    public float tiltSpeed = 360f;
    public float maxTiltAngle = 30f;
    public float tiltDeadzone = 0.2f;
    private Quaternion baseRotation;

    public void Start()
    {
        baseRotation = transform.rotation;
    }

    public void FixedUpdate()
    {
        Tilt();
    }

    public void OnAttack(InputValue value)
    {
        EventManager.TriggerRoundEnd(0); // TEMP
    }

    public void OnTilt(InputValue value)
    {
        tiltDirection = value.Get<float>();
    }

    private void Tilt()
    {
        float targetAngle = tiltDirection * maxTiltAngle;
        Quaternion targetRotation = baseRotation * Quaternion.Euler(0f, 0f, -targetAngle);
        float t = tiltSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
    }
}
