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
        if (fencer0.GetStateSnapshot(1).IsName("ParryLeft"))
        {
            Animator f1Animator = fencer1.GetComponent<Animator>();
            if (!f1Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
            {
                //f1Animator.Play("Parried", foilLayerIndex, 0f); //old method, no frame control
                motionPlayerP1.PlayScriptedMotion(parriedConfig, Vector3.zero);
            }
        }
        else if (fencer1.GetStateSnapshot(1).IsName("ParryLeft"))
        {
            Animator f0Animator = fencer0.GetComponent<Animator>();
            if (!f0Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
            {
                //f1Animator.Play("Parried", foilLayerIndex, 0f); //old method, no frame control
                motionPlayerP0.PlayScriptedMotion(parriedConfig, Vector3.zero);
            }
        }
    }
}
