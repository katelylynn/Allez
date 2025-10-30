using UnityEngine;

public class FencerColliderScript : MonoBehaviour
{

    public Mover mv;

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "FencerCollider")
            mv.SetForwardMovement(false);
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "FencerCollider")
            mv.SetForwardMovement(true);
    }
}
