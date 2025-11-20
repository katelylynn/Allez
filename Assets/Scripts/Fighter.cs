using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    public Transform ParryTracker;
    public float tiltSpeed = 5;
    public float parryForce = 3;

    private Animator anim;

    public float leftTiltPos = -5;
    public float rightTiltPos = 1.9f;
    public float unTiltPos = 0;

    private Coroutine currentTiltCoroutine;
    private Coroutine currentParryCoroutine;
    public GameObject foilAttackBox;
    //public bool foilHitBoxEnabled = true;

    ScriptedMotionPlayer motionPlayer;
    PlayerStamina stamina;
    [Header("Scripted Motion Configs")]
    public ScriptedMotionConfig attackConfig;
    public ScriptedMotionConfig parryLeftConfig;

    public void Start()
    {
        stamina = GetComponent<PlayerStamina>();
        anim = GetComponent<Animator>();
        if (motionPlayer == null)
            motionPlayer = GetComponent<ScriptedMotionPlayer>();
    }
    public void Attack()
    {
        if (stamina.ConsumeStamina(attackConfig.staminaCost))
            motionPlayer.PlayScriptedMotion(attackConfig, Vector3.zero);
    }

    public void OnAttack(InputValue value) => Attack();

    public void TiltLeft()
    {
        if (stamina.ConsumeStamina(parryLeftConfig.staminaCost))
            motionPlayer.PlayScriptedMotion(parryLeftConfig, Vector3.zero);
    }

    public void TiltRight()
    {
        //if (motionPlayer != null)
        //    motionPlayer.PlayScriptedMotion(parryRightConfig, Vector3.zero);
    }

    public void OnTilt(InputValue tiltDirection)
    {
        // need to check if any triggers are being set before doing this
        // can only not do this if player is lunging
        float tilt = tiltDirection.Get<float>();

        if (currentTiltCoroutine != null)
            StopCoroutine(currentTiltCoroutine);

        if (tilt == -1)
        {
            currentTiltCoroutine = StartCoroutine(DoTilt(leftTiltPos));
        }
        else if (tilt == 1)
        {
            currentTiltCoroutine = StartCoroutine(DoTilt(rightTiltPos));
        }
        else
        {
            currentTiltCoroutine = StartCoroutine(DoTilt(unTiltPos));
        }
    }

    public IEnumerator DoTilt(float targetLocalX)
    {
        float time = 0;
        Vector3 startPos = ParryTracker.localPosition;
        Vector3 targetPos = new Vector3(targetLocalX, startPos.y, startPos.z);

        while (time < 1)
        {
            ParryTracker.localPosition = Vector3.Lerp(startPos, targetPos, time);
            time += Time.deltaTime * tiltSpeed;
            yield return null;
        }

        ParryTracker.localPosition = targetPos; // Snap to final position
    }

    public void OnParry(InputValue parryDirection)
    {
        float parryDir = parryDirection.Get<float>();

        // can only do this if player is not attacking, lunging, or backdashing        
        if (currentParryCoroutine == null && ParryTracker.localPosition.x == 0 && parryDir != 0)
        {
            GameObject Rig1 = ParryTracker.parent.gameObject;
            Transform child = Rig1.transform.GetChild(0);
            MultiAimConstraint aimConstraint = child.GetComponent<MultiAimConstraint>();
            aimConstraint.weight = 0f;

            if (parryDir == -1 && stamina.ConsumeStamina(parryLeftConfig.staminaCost))
            {
                //parry left
                currentParryCoroutine = StartCoroutine(DoParry(-parryForce, true));
                GetComponent<PlayerAudioController>().PlaySwing();
            }
            else if (parryDir == 1 && stamina.ConsumeStamina(parryLeftConfig.staminaCost))
            {
                //parry right
                currentParryCoroutine = StartCoroutine(DoParry(parryForce, true));
                GetComponent<PlayerAudioController>().PlaySwing();
            }
        }
    }

    public IEnumerator DoParry(float direction, bool isReversing = false)
    {
        anim.SetBool("Parry", true);
        float time = 0;
        Vector3 startPos = ParryTracker.localPosition;

        // Instead of absolute position, use relative offset:
        Vector3 targetPos = startPos + new Vector3(direction, 0, 0);

        while (time < 1)
        {
            ParryTracker.localPosition = Vector3.Lerp(startPos, targetPos, time);
            time += Time.deltaTime * tiltSpeed;
            yield return null;
        }

        ParryTracker.localPosition = targetPos;
        currentParryCoroutine = null;

        // Ensures that the sword goes back to its original position
        if (isReversing)
        {
            currentParryCoroutine = StartCoroutine(DoParry(-direction));
        }

        if (currentParryCoroutine == null)
        {
            GameObject Rig1 = ParryTracker.parent.gameObject;
            Transform child = Rig1.transform.GetChild(0);
            MultiAimConstraint aimConstraint = child.GetComponent<MultiAimConstraint>();
            aimConstraint.weight = 1f;

            anim.SetBool("Parry", false);
        }
    }
}
