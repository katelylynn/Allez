using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private Fencer fencer0;
    private Fencer fencer1;

    // I'm considering a better way to organize all the animation layers and their indices
    private int foilLayerIndex = 1;

    public void Start()
    {
        EventManager.ParrySuccess += HandleParrySuccess;
    }

    public void Initialize( Fencer f0, Fencer f1 )
    {
        fencer0 = f0;
        fencer1 = f1;
    }

    private void HandleParrySuccess()
    {
        if ( fencer0.GetStateSnapshot( 1 ).IsName( "ParryLeft" ) )
        {
            Debug.Log( "fencer 0 parries Fencer 1!" );
            Animator f1Animator = fencer1.GetComponent<Animator>();
            if (!f1Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
                f1Animator.Play( "Parried", foilLayerIndex, 0f );
        }
        else if ( fencer1.GetStateSnapshot( 1 ).IsName( "ParryLeft" ) )
        {
            Debug.Log( "Fencer 1 parries Fencer 0!" );
            Animator f0Animator = fencer0.GetComponent<Animator>();
            if (!f0Animator.GetCurrentAnimatorStateInfo(foilLayerIndex).IsName("Parried"))
                f0Animator.Play( "Parried", foilLayerIndex, 0f );
        }
    }
}
