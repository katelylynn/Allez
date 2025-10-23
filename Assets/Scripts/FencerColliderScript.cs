using UnityEngine;

public class FencerColliderScript : MonoBehaviour
{

    public Mover mv;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("collider enter");
        if(collision.gameObject.tag == "FencerCollider")
            mv.SetForwardMovement(false);
    }

    private void OnTriggerExit(Collider collision)
    {
        Debug.Log("collider exit");
        if (collision.gameObject.tag == "FencerCollider")
            mv.SetForwardMovement(true);
    }
}
