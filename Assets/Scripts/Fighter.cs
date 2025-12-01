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

    private Animator anim;
    
    public float tiltSpeed = 5;
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
    public ScriptedMotionConfig parryConfig;

    public void Start()
    {
        stamina = GetComponent<PlayerStamina>();
        anim = GetComponent<Animator>();
        if (motionPlayer == null)
            motionPlayer = GetComponent<ScriptedMotionPlayer>();
    }
    public void Attack(float value)
    {
        if (anim.GetBool("Parry"))
            return;

        if (value == -1 && stamina.ConsumeStamina(attackConfig.staminaCost))
        {
            Debug.Log("Attack Left!");
        } else if (value == 1 && stamina.ConsumeStamina(attackConfig.staminaCost))
        {
            motionPlayer.PlayScriptedMotion(attackConfig, Vector3.zero);
        }
            
    }

    public void OnAttack(InputValue value) => Attack(value.Get<float>());

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
