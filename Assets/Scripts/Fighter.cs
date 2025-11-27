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

    // tilt params
    public float tiltSpeed = 5;
    public float leftTiltPos = -5;
    public float rightTiltPos = 1.9f;
    public float unTiltPos = 0;

    // coroutines
    private Coroutine currentTiltCoroutine;

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

    public void OnAttack(InputValue value) => Attack();

    public void Attack()
    {
        if (stamina.ConsumeStamina(attackConfig.staminaCost) && !anim.GetBool("Parry"))
            motionPlayer.PlayScriptedMotion(attackConfig, Vector3.zero);
    }

    public void OnTilt(InputValue tiltDirection) => Tilt(tiltDirection.Get<float>());

    public void Tilt(float tilt)
    {
        if (currentTiltCoroutine != null)
            StopCoroutine(currentTiltCoroutine);

        if (tilt == -1)
            currentTiltCoroutine = StartCoroutine(DoTilt(leftTiltPos));
        else if (tilt == 1)
            currentTiltCoroutine = StartCoroutine(DoTilt(rightTiltPos));
        else
            currentTiltCoroutine = StartCoroutine(DoTilt(unTiltPos));

    }

    private IEnumerator DoTilt(float targetLocalX)
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

    public void ResetSword()
    {
        //Reset weight to disable wide wrist movement
        GameObject Rig1 = ParryTracker.parent.gameObject;
        Transform child = Rig1.transform.GetChild(0);
        MultiAimConstraint aimConstraint = child.GetComponent<MultiAimConstraint>();
        aimConstraint.weight = 1f;

        //Reset the sphere game object to center, which resets sword to center
        ParryTracker.localPosition = new Vector3(unTiltPos, ParryTracker.localPosition.y, ParryTracker.localPosition.z);
    }
}
