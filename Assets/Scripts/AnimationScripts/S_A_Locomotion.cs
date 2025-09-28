using UnityEngine;

public class S_A_Locomotion : MonoBehaviour
{
    public float speed = 10.0f;

    private Animator animator;
    private Rigidbody rb;

    private bool isSprinting = false;

    private Vector2 direction = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = new Vector2(0.0f, Input.GetAxis( "Vertical" ) ).normalized;

        animator.SetFloat( "InputY", direction.y );
        animator.SetFloat( "InputX", direction.x );
    }

    void FixedUpdate()
    {
        // --- Movement ---
        //float targetSpeed = isSprinting ? runSpeed : walkSpeed;

        Vector3 move = new Vector3( direction.x, 0, direction.y ) * speed;

        // Keep existing vertical velocity (gravity)
        Vector3 velocity = rb.linearVelocity;

        if (velocity != Vector3.zero) Debug.Log( "linear velocity: " + rb.linearVelocity );
        velocity.x = move.x;
        velocity.z = move.z;

        rb.linearVelocity = velocity;
        if (velocity != Vector3.zero) Debug.Log( "new linear velocity: " + rb.linearVelocity );
    }
}
