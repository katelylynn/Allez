using UnityEngine;

public class FoilAttack : MonoBehaviour
{
    public Fencer fencer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Torso"))
        {
            EventManager.TriggerRoundEnd(fencer.fencerId);
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("trigger exit");
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}
