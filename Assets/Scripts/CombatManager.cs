using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private Fencer fencer0;
    private Fencer fencer1;

    private int foilLayerIndex = 1;

    ScriptedMotionPlayer motionPlayerP0;
    ScriptedMotionPlayer motionPlayerP1;
    [Header("Scripted Motion Configs")]
    public ScriptedMotionConfig parriedConfig;

    public int parriedLungeStaminaCost = 50;
    public int parriedAttackStaminaCost = 25;
    public int succPStaminaGain = 20;
    public void Awake()
    {
    }
    public void Start()
    {
        EventManager.ParrySuccess += HandleParrySuccess;
    }

    public void OnDestroy()
    {
        EventManager.ParrySuccess -= HandleParrySuccess;
    }

    public void Initialize(Fencer f0, Fencer f1)
    {
        fencer0 = f0;
        fencer1 = f1;
        motionPlayerP0 = f0.GetComponent<ScriptedMotionPlayer>();
        motionPlayerP1 = f1.GetComponent<ScriptedMotionPlayer>();
    }

    private void HandleParrySuccess()
    {
        Animator f0Animator = fencer0.GetComponent<Animator>();
        Animator f1Animator = fencer1.GetComponent<Animator>();

        bool f0IsParrying = f0Animator.GetBool("Parry");
        bool f1IsParrying = f1Animator.GetBool("Parry");

        if (f0IsParrying)
        {
            // fencer0 parried apply Parried animation to fencer1
            if (!f1Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
            {
                //additional stamina changes on succ p
                fencer0.GetComponent<PlayerStamina>().AddStamina(succPStaminaGain);
                if (f1Animator.GetCurrentAnimatorStateInfo(0).IsName("LungeSmooth"))
                {
                    fencer1.GetComponent<PlayerStamina>().ConsumeStaminaWhenParried(parriedLungeStaminaCost);
                }else if (f1Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Attack"))
                {
                    fencer1.GetComponent<PlayerStamina>().ConsumeStaminaWhenParried(parriedAttackStaminaCost);
                }
                fencer1.GetComponent<ScriptedMotionPlayer>().StopCurrentMotion();
                motionPlayerP1.PlayScriptedMotion(parriedConfig, Vector3.zero);
                EventManager.TriggerActionTaken(OpponentMove.AIParried);
            }
        }
        else if (f1IsParrying)
        {
            // fencer1 parried apply Parried animation to fencer0
            if (!f0Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
            { 
                //additional stamina changes on succ p
                fencer1.GetComponent<PlayerStamina>().AddStamina(succPStaminaGain);
                if (f0Animator.GetCurrentAnimatorStateInfo(0).IsName("LungeSmooth"))
                {
                    Debug.Log("consuming stamina for parried lunge of f0");
                    fencer0.GetComponent<PlayerStamina>().ConsumeStaminaWhenParried(parriedLungeStaminaCost);
                }
                else if (f0Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Attack"))
                {
                    Debug.Log("consuming stamina for parried attack of f0");
                    fencer0.GetComponent<PlayerStamina>().ConsumeStaminaWhenParried(parriedAttackStaminaCost);
                }

                fencer0.GetComponent<ScriptedMotionPlayer>().StopCurrentMotion();
                motionPlayerP0.PlayScriptedMotion(parriedConfig, Vector3.zero);
            EventManager.TriggerActionTaken(OpponentMove.OpponentParried);
            }
        }
    }
}
