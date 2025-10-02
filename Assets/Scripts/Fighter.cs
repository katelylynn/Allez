using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    [Header("Attack Settings")]
    private Transform sword;
    private bool goingForward;
    private bool goingBackward;
    private Vector3 newSwordPosition;
    private Vector3 oldSwordPosition;
    public float attackDistance = 2f;
    public float moveSpeed = 0.1f;

    [Header("Tilt Settings")]
    private float tiltDirection;
    public float tiltSpeed = 360f;
    public float maxTiltAngle = 30f;
    private Quaternion baseRotation;

    public void Start()
    {
        sword = transform.Find("Sword");
        baseRotation = transform.rotation;
    }

    public void FixedUpdate()
    {
        Attack();
        Tilt();
    }

    public void OnAttack(InputValue value)
    {
        oldSwordPosition = sword.position;
        newSwordPosition = sword.position + sword.forward * attackDistance;
        goingForward = true;
    }

    private void Attack()
    {
        if (goingForward)
        {
            sword.position = sword.position + sword.forward * moveSpeed;

            if (Vector3.Dot(sword.position - oldSwordPosition, sword.forward) >= attackDistance)
            {
                goingForward = false;
                goingBackward = true;
            }
        }
        else if (goingBackward)
        {
            sword.position = sword.position - sword.forward * moveSpeed;

            if (Vector3.Dot(sword.position - oldSwordPosition, sword.forward) <= 0f)
            {
                goingBackward = false;
            }
        }
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
