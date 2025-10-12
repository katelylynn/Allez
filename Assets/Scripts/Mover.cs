using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    private string walkAnimationParam = "InputY";

    [Header("Movement Settings")]
    private float moveAmount;
    public float acceleration = 2f;
    public float deceleration = 10f;
    public float maxSpeed = 10f;

    [Header("Dash Settings")]
    public float lungeStrength = 50f;
    public float backdashStrength = 20f;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    public void FixedUpdate()
    {
        Move();
    }

    public void OnMovement(InputValue value)
    {
        moveAmount = value.Get<float>();

        if (moveAmount == -1)
            anim.SetFloat(walkAnimationParam, 0.5f);
        else
            anim.SetFloat(walkAnimationParam, moveAmount);
    }

    private void Move()
    {
        Vector3 localZ = new Vector3(0f, 0f, moveAmount);
        rb.AddRelativeForce(localZ * acceleration, ForceMode.VelocityChange);

        if (Mathf.Abs(moveAmount) > 0f && rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        else
            rb.AddForce(-rb.linearVelocity * deceleration, ForceMode.Acceleration);
    }

    public void OnLunge(InputValue value)
    {
        Debug.Log("Lunging");
        anim.SetTrigger("Lunge");

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(transform.forward * lungeStrength, ForceMode.VelocityChange);
    }

    public void OnBackdash(InputValue value)
    {
        Debug.Log("Backdashing");
        anim.SetTrigger("Backdash");

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(-transform.forward * backdashStrength, ForceMode.VelocityChange);
    }
}
