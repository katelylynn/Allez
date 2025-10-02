using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    private Rigidbody rb;
    private float moveAmount;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        Move();
    }

    public void OnMovement(InputValue value)
    {
        moveAmount = value.Get<float>();
    }

    private void Move()
    {
        rb.AddForce(new Vector3(0f, 0f, moveAmount), ForceMode.VelocityChange);
    }

    public void OnLunge(InputValue value)
    {
        Debug.Log("Lunge pressed");
    }

    public void OnBackdash(InputValue value)
    {
        Debug.Log("Backdash pressed");
    }
}
