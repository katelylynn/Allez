using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class S_A_RigWeightController : MonoBehaviour
{
    private Rig foilRig;
    private Animator animator;

    private int foilLayerIndex = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foilRig = transform.GetComponentInChildren<Rig>( true );
        animator = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( animator.GetCurrentAnimatorStateInfo( foilLayerIndex ).IsName( "ParryLeft" ) || animator.GetCurrentAnimatorStateInfo( foilLayerIndex ).IsName( "Parried" ) )
        {
            foilRig.weight = 0f;
        }
        else
        {
            foilRig.weight = 1f;
        }
    }
}
