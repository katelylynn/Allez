using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;
    PlayerStamina stamina;

    [Header("Movement Settings")]
    private float moveAmount = 0f;
    public float acceleration = 2f;
    public float deceleration = 10f;
    public float maxSpeed = 10f;
    public bool allowForwardMovement = true;
    private string walkAnimationParam = "InputY";

    ScriptedMotionPlayer motionPlayer;
    [Header("Scripted Motions")]
    public ScriptedMotionConfig lungeConfig;
    public ScriptedMotionConfig backdashConfig;

    public GameObject foilTipHitBox; //this can be used to disabled foiltip during startup and recovery, not used currently

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        stamina = GetComponent<PlayerStamina>();
        if (motionPlayer == null)
        {
            motionPlayer = GetComponent<ScriptedMotionPlayer>();
        }
    }

    private void OnDisable()
    {
        anim.ResetTrigger("Lunge");
        anim.ResetTrigger("Backdash");
        anim.speed = 1f;
    }

    private void Update()
    {
        if (moveAmount == -1)
            allowForwardMovement = true;
        if (moveAmount != 0 && allowForwardMovement || moveAmount == -1)
        {
            Move();
        }
        else if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
    public void Lunge()
    {
        if (motionPlayer == null)
        {
            Debug.LogWarning("[Mover] motionPlayer is NULL, can't lunge.");
            return;
        }

        if (motionPlayer.isPlaying)
        {
            Debug.Log("[Mover] motionPlayer is already playing, ignoring lunge.");
            return;
        }
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack")) return;
        
        if (anim.GetBool("Parry")) return;
        
        if(stamina.ConsumeStamina(lungeConfig.staminaCost))
            motionPlayer.PlayScriptedMotion(lungeConfig, transform.forward);
    }

    public void OnLunge(InputValue value)
    {
        Lunge();
        EventManager.TriggerActionTaken(OpponentMove.Lunge);
    }

    public void OnBackdash(InputValue value)
    {
        Backdash();
        EventManager.TriggerActionTaken(OpponentMove.Backdash);
    }

    public void Backdash()
    {
        if (motionPlayer == null) return;

        if (motionPlayer.isPlaying) return;

        if (anim.GetBool("Parry")) return;

        if(stamina.ConsumeStamina(backdashConfig.staminaCost))
            motionPlayer.PlayScriptedMotion(backdashConfig, -transform.forward);
    }

    public void SetMoveAmount(float ma)
    {
        ma = (float)Math.Round(ma);
        if (ma != -1 && ma != 0 && ma != 1)
            Debug.Log($"{ma} Mover.cs SetMoveAmount: not a valid move amount!");

        moveAmount = ma;

        if (moveAmount == -1)
            anim.SetFloat(walkAnimationParam, 0.5f);
        else
            anim.SetFloat(walkAnimationParam, moveAmount);
    }
    public void OnMovement(InputValue value)
    {
        SetMoveAmount(value.Get<float>());
    }

    private void Move()
    {
        Vector3 localZ = new Vector3(0f, 0f, moveAmount);
        rb.AddRelativeForce(localZ * acceleration, ForceMode.VelocityChange);

        if (Mathf.Abs(moveAmount) > 0f && rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        else
            rb.AddForce(-rb.linearVelocity * deceleration, ForceMode.Acceleration);
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
