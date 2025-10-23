using UnityEngine;

public class S_A_LayerWeightController : MonoBehaviour
{
    private Animator animator;
    private int baseLayerIndex = 0;
    private int foiLayerIndex = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( animator.GetCurrentAnimatorStateInfo( baseLayerIndex ).IsName( "Lunge Center" ) )
        {
            animator.SetLayerWeight( foiLayerIndex, 0.0f );
        }
        else
        {
            animator.SetLayerWeight( foiLayerIndex, 1.0f );
        }
    }
}
