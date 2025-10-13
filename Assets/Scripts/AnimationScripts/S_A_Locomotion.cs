using System;
using UnityEngine;

public class S_A_Locomotion : MonoBehaviour
{
    public float walkSpeed = 3.0f;
    public float runSpeed = 7.0f;

    public KeyCode moveForwardKey = KeyCode.W;
    public KeyCode moveBackwardKey = KeyCode.S;

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
        direction = Vector2.zero;

        if (Input.GetKey(moveForwardKey))
        {
            direction.y = 1.0f;
            animator.SetFloat("InputY", direction.y);
        }
        else if (Input.GetKey(moveBackwardKey))
        {
            direction.y = -1.0f;
            animator.SetFloat("InputY", 0.5f);
        } else {
            animator.SetFloat("InputY", direction.y);
        }

        if (direction != Vector2.zero)
        {
            Debug.Log(direction);
        }

        //isSprinting = Input.GetKey( KeyCode.LeftShift );
        //animator.SetBool( "IsSprinting", isSprinting );
    }

    void FixedUpdate()
    {
        // --- Movement ---
        float targetSpeed = isSprinting ? runSpeed : walkSpeed;

        Vector3 move = new Vector3(direction.x, 0, direction.y) * targetSpeed;

        // Keep existing vertical velocity (gravity)
        Vector3 velocity = rb.linearVelocity;
        velocity.x = move.x;
        velocity.z = move.z;

        rb.linearVelocity = velocity;
    }
}
