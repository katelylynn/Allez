using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class Fighter : MonoBehaviour
{
    public Transform ParryTracker;

    private Animator anim;
    
    public float tiltSpeed = 5;
    public float leftTiltPos = -10;
    public float rightTiltPos = 1.9f;
    public float unTiltPos = 0;
    public float tiltFramePercentage = 0.2f;
    private int OGStartupFrames = 0;
    private int OGRecoveryFrames = 0;

    private Coroutine currentAttackLeftCoroutine;
    private Coroutine currentParryCoroutine;
    public GameObject foilAttackBox;
    //public bool foilHitBoxEnabled = true;

    ScriptedMotionPlayer motionPlayer;
    PlayerStamina stamina;
    [Header("Scripted Motion Configs")]
    public ScriptedMotionConfig attackConfig;
    public ScriptedMotionConfig parryConfig;

    public void Start()
    {
        stamina = GetComponent<PlayerStamina>();
        anim = GetComponent<Animator>();
        if (motionPlayer == null)
            motionPlayer = GetComponent<ScriptedMotionPlayer>();
    }

    public void OnAttack(InputValue value) => Attack(value.Get<float>());
    
    public void Attack(float value)
    {
        if (anim.GetBool("Parry") || motionPlayer.isPlaying)
            return;

        if (value == -1 && stamina.ConsumeStamina(attackConfig.staminaCost) && currentAttackLeftCoroutine == null)
        {
            OGStartupFrames = attackConfig.startupFrames;
            OGRecoveryFrames = attackConfig.recoveryFrames;
            currentAttackLeftCoroutine = StartCoroutine(DoAttackLeft(leftTiltPos));
        }
        
        else if (value == 1 && stamina.ConsumeStamina(attackConfig.staminaCost))
            motionPlayer.PlayScriptedMotion(attackConfig, Vector3.zero);
    }

    private IEnumerator DoAttackLeft(float targetLocalX, bool finishedAttack = false)
    {
        Vector3 startPos = ParryTracker.localPosition;
        Vector3 targetPos = new Vector3(targetLocalX, startPos.y, startPos.z);

        float frameCount = 0;
        
        // Ensures that the tilt motion is also part of the startup/recovery frames
        if (!finishedAttack)
        {
            frameCount = Mathf.CeilToInt(attackConfig.startupFrames * tiltFramePercentage);
            attackConfig.startupFrames = Mathf.FloorToInt(attackConfig.startupFrames * (1-tiltFramePercentage));
        } else if(finishedAttack)
        {
            frameCount = Mathf.CeilToInt(attackConfig.recoveryFrames * tiltFramePercentage);
            attackConfig.recoveryFrames = Mathf.FloorToInt(attackConfig.recoveryFrames * (1-tiltFramePercentage));
        }
        
        for (int i = 0; i < frameCount; i++)
        {
            float t = (float)i / (frameCount - 1);  // normalized 0 → 1
            ParryTracker.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;   // wait 1 frame
            if (motionPlayer.isPlaying)
            {
                //reset parry tracker if another animation starts playing
                ParryTracker.localPosition = new Vector3(unTiltPos, startPos.y, startPos.z);
                attackConfig.startupFrames = OGStartupFrames;
                attackConfig.recoveryFrames = OGRecoveryFrames;
                currentAttackLeftCoroutine = null;
                yield break;
            }
        }

        ParryTracker.localPosition = targetPos; // Snap to final position

        if (!finishedAttack)
            motionPlayer.PlayScriptedMotion(attackConfig, Vector3.zero);
        
        while(motionPlayer.isPlaying)
            yield return null;

        if(!finishedAttack)
            currentAttackLeftCoroutine = StartCoroutine(DoAttackLeft(unTiltPos, true));

        if (finishedAttack)
        {
            // Restore the frame amounts to their original level
            attackConfig.startupFrames = OGStartupFrames;
            attackConfig.recoveryFrames = OGRecoveryFrames;
        }
        currentAttackLeftCoroutine = null;
    }

    public void OnParry(InputValue parryDirection) => Parry(parryDirection.Get<float>());

    public void Parry(float parryDir)
    {
        if (motionPlayer.isPlaying) return;
        
        // can only do this if player is not attacking, lunging, or backdashing        
        if (currentParryCoroutine == null && ParryTracker.localPosition.x == 0 && parryDir != 0)
        {
            GameObject Rig1 = ParryTracker.parent.gameObject;
            Transform child = Rig1.transform.GetChild(0);
            MultiAimConstraint aimConstraint = child.GetComponent<MultiAimConstraint>();

            if (parryDir == -1 && stamina.ConsumeStamina(parryConfig.staminaCost))
            {
                //parry left
                aimConstraint.weight = 0f;
                currentParryCoroutine = StartCoroutine(DoParry(-parryConfig.distance, true));
                GetComponent<PlayerAudioController>().PlaySwing();
            }
            else if (parryDir == 1 && stamina.ConsumeStamina(parryConfig.staminaCost))
            {
                //parry right
                aimConstraint.weight = 0f;
                currentParryCoroutine = StartCoroutine(DoParry(parryConfig.distance, true));
                GetComponent<PlayerAudioController>().PlaySwing();
            }
        }
    }

    private IEnumerator DoParry(float direction, bool isReversing = false)
    {
        anim.SetBool("Parry", true);
        Vector3 startPos = ParryTracker.localPosition;

        // Instead of absolute position, use relative offset:
        Vector3 targetPos = startPos + new Vector3(direction, 0, 0);

        float frameCount = isReversing ? parryConfig.activeFrames : parryConfig.recoveryFrames;
        for (int i = 0; i < frameCount; i++)
        {
            float t = (float)i / (frameCount - 1);  // normalized 0 → 1
            ParryTracker.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;   // wait 1 frame
        }

        ParryTracker.localPosition = targetPos;
        currentParryCoroutine = null;

        // Ensures that the sword goes back to its original position
        if (isReversing)
            currentParryCoroutine = StartCoroutine(DoParry(-direction));

        if (currentParryCoroutine == null)
        {
            GameObject Rig1 = ParryTracker.parent.gameObject;
            Transform child = Rig1.transform.GetChild(0);
            MultiAimConstraint aimConstraint = child.GetComponent<MultiAimConstraint>();
            aimConstraint.weight = 1f;

            anim.SetBool("Parry", false);
        }
    }

    public void ResetSword()
    {
        //Reset weight to disable wide wrist movement
        GameObject Rig1 = ParryTracker.parent.gameObject;
        Transform child = Rig1.transform.GetChild(0);
        MultiAimConstraint aimConstraint = child.GetComponent<MultiAimConstraint>();
        aimConstraint.weight = 1f;

        //Reset the sphere game object to center, which resets sword to center
        ParryTracker.localPosition = new Vector3(unTiltPos, ParryTracker.localPosition.y, ParryTracker.localPosition.z);

        currentParryCoroutine = null;
    }
}
