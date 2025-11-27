using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    private Animator anim;
    public GameObject foilAttackBox;

    // parry params
    public Transform ParryTracker;
    private Coroutine currentParryCoroutine;

    public float parrySpeed = 5;
    public float leftParryPos = -5;
    public float rightParryPos = 1.9f;
    public float noParryPos = 0;

    public float currParryDirection = 0;
    public bool isParrying = false;

    public MultiAimConstraint armConstraint;

    // util script references
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

    public void FixedUpdate()
    {
        if (ParryTracker.localPosition.x < leftParryPos / 2)
            currParryDirection = -1;
        else if (ParryTracker.localPosition.x > rightParryPos / 2)
            currParryDirection = 1;
        else
            currParryDirection = 0;
    }

    public void OnAttack(InputValue value) => Attack();

    public void Attack()
    {
        if (!anim.GetBool("Parry") && stamina.ConsumeStamina(attackConfig.staminaCost))
            motionPlayer.PlayScriptedMotion(attackConfig, Vector3.zero);
    }

    public void OnParry(InputValue parryDirection) => Parry(parryDirection.Get<float>());

    public void Parry(float parryDir)
    {
        if (currentParryCoroutine != null)
            StopCoroutine(currentParryCoroutine);

        // can only do this if player is not attacking, lunging, or backdashing        
        if (parryDir == -1 && stamina.ConsumeStamina(parryConfig.staminaCost))
        {
            //parry left
            currentParryCoroutine = StartCoroutine(DoParry(leftParryPos));
            GetComponent<PlayerAudioController>().PlaySwing();
        }
        else if (parryDir == 1 && stamina.ConsumeStamina(parryConfig.staminaCost))
        {
            //parry right
            currentParryCoroutine = StartCoroutine(DoParry(rightParryPos));
            GetComponent<PlayerAudioController>().PlaySwing();
        }
        else
        {
            currentParryCoroutine = StartCoroutine(DoParry(noParryPos));
        }
    }

    private IEnumerator DoParry(float targetLocalX)
    {
        isParrying = true;
        float time = 0;
        Vector3 startPos = ParryTracker.localPosition;
        Vector3 targetPos = new Vector3(targetLocalX, startPos.y, startPos.z);

        while (time < 1)
        {
            ParryTracker.localPosition = Vector3.Lerp(startPos, targetPos, time);
            time += Time.deltaTime * parrySpeed;
            yield return null;
        }

        ParryTracker.localPosition = targetPos; // Snap to final position
        isParrying = false;
    }

    public void ResetSword()
    {
        armConstraint.weight = 1f;

        //Reset the sphere game object to center, which resets sword to center
        ParryTracker.localPosition = new Vector3(noParryPos, ParryTracker.localPosition.y, ParryTracker.localPosition.z);
    }
}
