using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    [Header("Attack Settings")]
    private Transform foil;
    private bool goingForward;
    private bool goingBackward;
    private Vector3 newFoilPosition;
    private Vector3 oldFoilPosition;
    public float attackDistance = 2f;
    public float moveSpeed = 0.1f;

    [Header("Tilt Settings")]
    private float tiltDirection;
    public float tiltSpeed = 360f;
    public float maxTilt = 1f;
    private Quaternion baseRotation;

    public void Start()
    {
        foil = transform.Find("Foil");
        baseRotation = transform.rotation;
    }

    public void FixedUpdate()
    {
        Attack();
        Tilt();
    }

    public void OnAttack(InputValue value)
    {
        oldFoilPosition = foil.position;
        newFoilPosition = foil.position + foil.forward * attackDistance;
        goingForward = true;
    }

    private void Attack()
    {
        if (goingForward)
        {
            foil.position = foil.position + foil.forward * moveSpeed;

            if (Vector3.Dot(foil.position - oldFoilPosition, foil.forward) >= attackDistance)
            {
                goingForward = false;
                goingBackward = true;
            }
        }
        else if (goingBackward)
        {
            foil.position = foil.position - foil.forward * moveSpeed;

            if (Vector3.Dot(foil.position - oldFoilPosition, foil.forward) <= 0f)
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
            targetX = foil.localPosition.x + tiltDirection * tiltSpeed * Time.fixedDeltaTime;
        else
            targetX = 0f;

        float newX = Mathf.MoveTowards(foil.localPosition.x, targetX, tiltSpeed * Time.fixedDeltaTime);

        newX = Mathf.Clamp(newX, -maxTilt, maxTilt);

        foil.localPosition = new Vector3(newX, foil.localPosition.y, foil.localPosition.z);
    }
}
