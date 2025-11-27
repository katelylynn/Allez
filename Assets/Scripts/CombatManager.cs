using UnityEngine;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    private Fencer fencer0;
    private Fencer fencer1;

    private Fighter fighter0;
    private Fighter fighter1;

    Animator f0Animator;
    Animator f1Animator;

    ScriptedMotionPlayer motionPlayerP0;
    ScriptedMotionPlayer motionPlayerP1;

    [Header("Scripted Motion Configs")]
    public ScriptedMotionConfig parriedConfig;

    private int foilLayerIndex = 1;

    public void Start()
    {
        EventManager.ParrySuccess += HandleParrySuccess;

        fighter0 = fencer0.GetComponent<Fighter>();
        fighter1 = fencer1.GetComponent<Fighter>();

        f0Animator = fencer0.GetComponent<Animator>();
        f1Animator = fencer1.GetComponent<Animator>();
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
        if (fighter0.isParrying && !f1Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
        {
            fencer1.GetComponent<ScriptedMotionPlayer>().StopCurrentMotion();
            StartCoroutine(DoParried(motionPlayerP1, fighter1));
        }
        else if (fighter1.isParrying && !f0Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
        {
            fencer0.GetComponent<ScriptedMotionPlayer>().StopCurrentMotion();
            StartCoroutine(DoParried(motionPlayerP0, fighter0));
        }
    }

    private IEnumerator DoParried(ScriptedMotionPlayer smp, Fighter fighter)
    {
        smp.PlayScriptedMotion(parriedConfig, Vector3.zero);

        Debug.Log("started");

        fighter.armConstraint.weight = 0f;

        while (smp.isPlaying)
        {
            yield return null;
        }

        fighter.armConstraint.weight = 1f;

        Debug.Log("finished");
    }
}
