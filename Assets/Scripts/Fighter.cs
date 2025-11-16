using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    private Animator anim;
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
        float dir = tiltDirection.Get<float>();
        if (dir == -1) TiltLeft();
        else if (dir == 1) TiltRight();
    }
    private void OnValidate()
    {
        //foilAttackBox.SetActive(foilHitBoxEnabled);
    }
    private void OnDisable()
    {
        if(anim != null)
            anim.speed = 1f;
        //if (foilAttackBox != null)
        //    foilAttackBox.SetActive(false);
    }
}
