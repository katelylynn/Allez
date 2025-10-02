using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    private Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMovement(InputValue value)
    {
        float amount = value.Get<float>();
        Debug.Log($"Movement axis value: {amount}");
        rb.AddForce(new Vector3(0f, 0f, amount), ForceMode.VelocityChange);
    }

    public void OnLunge(InputValue value)
    {
        if (value.isPressed)
            Debug.Log("Lunge pressed");
        else
            Debug.Log("Lunge released");
    }

    public void OnBackdash(InputValue value)
    {
        if (value.isPressed)
            Debug.Log("Backdash pressed");
        else
            Debug.Log("Backdash released");
    }
}
