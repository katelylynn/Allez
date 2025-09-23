using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputActions inputActions;
    private InputAction movement;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputActions();
        if(gameObject.tag == "P1")
        {
            movement = inputActions.Player.P1Movement;
        }
        if(gameObject.tag == "P2")
        {
            movement = inputActions.Player.P2Movement;
        }
    }

    private void OnEnable()
    {
        movement.Enable();

    }

    private void FixedUpdate()
    {
        Vector2 v2 = movement.ReadValue<Vector2>();
        Vector3 v3 = new Vector3(v2.x, 0 , v2.y);

        rb.AddForce(v3, ForceMode.VelocityChange);
    }
}
