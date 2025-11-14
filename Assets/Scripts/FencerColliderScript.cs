using UnityEngine;

public class FencerColliderScript : MonoBehaviour
{

    public Mover mv;

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("trigger");
        if (collision.gameObject.tag == "FencerCollider")
        {
            mv.ZeroVelocity();
            mv.SetForwardMovement(false);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        Debug.Log("Left trigger");
        if (collision.gameObject.tag == "FencerCollider")
        {
            mv.SetForwardMovement(true);
            mv.ZeroVelocity();
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "FencerCollider")
        {
            Debug.Log("Enter collision");
            mv.SetForwardMovement(false);
        }
    }

    private void OnCollisonExit(Collision collision)
    {
        if (collision.gameObject.tag == "FencerCollider")
        {
            Debug.Log("Left collision");
            mv.SetForwardMovement(true);
        }
    }
}
