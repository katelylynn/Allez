using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private Fencer fencer0;
    private Fencer fencer1;

    private int foilLayerIndex = 1;

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
                f1Animator.Play("Parried", foilLayerIndex, 0f);
        }
        else if (f1IsParrying)
        {
            // fencer1 parried apply Parried animation to fencer0
            if (!f0Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
                f0Animator.Play("Parried", foilLayerIndex, 0f);
        }
    }
}
