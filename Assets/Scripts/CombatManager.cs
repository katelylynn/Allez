/*
    Combat Manager
    Handles interactions between fencers, specifically with parry but open to extension.
*/

using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private int foilLayerIndex = 1;

    // fencer references
    private Fencer fencer0;
    private Fencer fencer1;
    private Animator f0Animator;
    private Animator f1Animator;
    bool f0IsParrying = false;
    bool f1IsParrying = false;

    // motion configs
    public ScriptedMotionPlayer motionPlayerP0;
    public ScriptedMotionPlayer motionPlayerP1;
    public ScriptedMotionConfig parriedConfig;

    // stamina params
    public int parriedLungeStaminaCost = 50;
    public int parriedAttackStaminaCost = 25;
    public int succPStaminaGain = 20;

    public void Start() => EventManager.ParrySuccess += HandleParrySuccess;
    public void OnDestroy() => EventManager.ParrySuccess -= HandleParrySuccess;

    public void Initialize(Fencer f0, Fencer f1)
    {
        fencer0 = f0;
        fencer1 = f1;
        f0Animator = fencer0.GetComponent<Animator>();
        f1Animator = fencer1.GetComponent<Animator>();
        motionPlayerP0 = f0.GetComponent<ScriptedMotionPlayer>();
        motionPlayerP1 = f1.GetComponent<ScriptedMotionPlayer>();
    }

    private void HandleParrySuccess()
    {
        f0IsParrying = f0Animator.GetBool("Parry");
        f1IsParrying = f1Animator.GetBool("Parry");

        // based on which fencer is parrying...
        if (f0IsParrying)
        {
            // check if already being parried...
            if (!f1Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
            {
                // parrier gets additional stamina on succ p
                fencer0.GetComponent<StaminaController>().AddStamina(succPStaminaGain);

                // consume attacking fencer's stamina based on what move performed
                if (f1Animator.GetCurrentAnimatorStateInfo(0).IsName("LungeSmooth"))
                    fencer1.GetComponent<StaminaController>().ConsumeStaminaWhenParried(parriedLungeStaminaCost);
                else if (f1Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Attack"))
                    fencer1.GetComponent<StaminaController>().ConsumeStaminaWhenParried(parriedAttackStaminaCost);

                // stop attack and play parry
                fencer1.GetComponent<ScriptedMotionPlayer>().StopCurrentMotion();
                motionPlayerP1.PlayScriptedMotion(parriedConfig, Vector3.zero);
                EventManager.TriggerActionTaken(OpponentMove.AIParried);
            }
        }
        else if (f1IsParrying)
        {
            // check if already being parried...
            if (!f0Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
            { 
                // parrier gets additional stamina on succ p
                fencer1.GetComponent<StaminaController>().AddStamina(succPStaminaGain);

                // consume attacking fencer's stamina based on what move performed
                if (f0Animator.GetCurrentAnimatorStateInfo(0).IsName("LungeSmooth"))
                    fencer0.GetComponent<StaminaController>().ConsumeStaminaWhenParried(parriedLungeStaminaCost);
                else if (f0Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Attack"))
                    fencer0.GetComponent<StaminaController>().ConsumeStaminaWhenParried(parriedAttackStaminaCost);

                // stop attack and play parry
                fencer0.GetComponent<ScriptedMotionPlayer>().StopCurrentMotion();
                motionPlayerP0.PlayScriptedMotion(parriedConfig, Vector3.zero);
                EventManager.TriggerActionTaken(OpponentMove.OpponentParried);
            }
        }
    }
}
