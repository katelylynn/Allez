using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;
    private Rigidbody rigidBody;

    public float jumpForce = 5f;
    private float verticalVelocity = 0f;
    private float gravity = -9.81f;
    private bool isFallen = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.isKinematic = true; // CharacterController handles movement until the character falls.

        if (animator == null)
            Debug.LogError("No Animator component found on " + gameObject.name);
    }

    void Update()
    {
        if (isFallen) return;
        if (animator == null) return;

        // Space = Jump (only if grounded)
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            animator.SetTrigger("Jump");
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Left Mouse = Attack
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }

        // Space + S = Retreat (only if grounded)
        if (Input.GetKey(KeyCode.Space) && Input.GetKeyDown(KeyCode.S) && controller.isGrounded)
        {
            animator.SetTrigger("BigRetreat");
        }

        // Right Mouse = Take Hit and Fall
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("HitAndFall");

            // When character falls, switch from CharacterController to RigidBody (physics) to allow the character to fall onto the ground.
            controller.enabled = false;
            rigidBody.isKinematic = false;
            isFallen = true;
        }

        if (controller.enabled)
        {
            // Apply gravity & move character
            if (controller.isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f; // small downward force keeps grounded
            }

            verticalVelocity += gravity * Time.deltaTime;
            Vector3 move = new Vector3(0, verticalVelocity, 0);
            controller.Move(move * Time.deltaTime);
        }
    }
}
