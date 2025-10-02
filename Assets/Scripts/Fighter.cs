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
    public float maxTilt = 1f;
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
        float targetX;

        if (Mathf.Abs(tiltDirection) > 0.01f)
            targetX = sword.localPosition.x + tiltDirection * tiltSpeed * Time.fixedDeltaTime;
        else
            targetX = 0f;

        float newX = Mathf.MoveTowards(sword.localPosition.x, targetX, tiltSpeed * Time.fixedDeltaTime);

        newX = Mathf.Clamp(newX, -maxTilt, maxTilt);

        sword.localPosition = new Vector3(newX, sword.localPosition.y, sword.localPosition.z);
    }
}
