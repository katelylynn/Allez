using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    [Header("Movement Settings")]
    private float moveAmount = 0f;
    public float acceleration = 2f;
    public float deceleration = 10f;
    public float maxSpeed = 10f;
    public bool allowForwardMovement = true;
    private string walkAnimationParam = "InputY";

    [Header("Dash Settings")]
    public float lungeStrength = 50f;
    public float backdashStrength = 20f;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (moveAmount == -1)
            allowForwardMovement = true;
        if (moveAmount != 0 && allowForwardMovement || moveAmount == -1)
        {
            Move();
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
    public void FixedUpdate()
    {
        
    }

    public void OnMovement(InputValue value)
    {
        SetMoveAmount(value.Get<float>());
    }

    public void SetMoveAmount(float ma)
    {
        if (ma != -1 && ma != 0 && ma != 1)
            Debug.Log("Mover.cs SetMoveAmount: not a valid move amount!");

        moveAmount = ma;

        if (moveAmount == -1)
            anim.SetFloat(walkAnimationParam, 0.5f);
        else
            anim.SetFloat(walkAnimationParam, moveAmount);
    }

    private void Move()
    {
        //Debug.Log("moving now");
        Vector3 localZ = new Vector3(0f, 0f, moveAmount);
        rb.AddRelativeForce(localZ * acceleration, ForceMode.VelocityChange);

        if (Mathf.Abs(moveAmount) > 0f && rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        else
            rb.AddForce(-rb.linearVelocity * deceleration, ForceMode.Acceleration);
    }

    public void Lunge()
    {
        anim.SetTrigger("Lunge");
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(transform.forward * lungeStrength, ForceMode.Acceleration);
    }

    public void OnLunge(InputValue value)
    {
        Lunge();
    }

    public void Backdash()
    {
        anim.SetTrigger("Backdash");
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(-transform.forward * backdashStrength, ForceMode.VelocityChange);
    }

    public void OnBackdash(InputValue value)
    {
        Backdash();
    }

    public void SetForwardMovement(bool b)
    {
        allowForwardMovement = b;
    }

    public void ZeroVelocity()
    {
        rb.linearVelocity = Vector3.zero;
    }
}
