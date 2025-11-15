using UnityEngine;

public class FoilAttack : MonoBehaviour
{
    public Fencer fencer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Torso"))
        {
            EventManager.TriggerRoundEnd(fencer.fencerId);
            //gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
            gameObject.GetComponentInChildren<ParticleSystem>().Play();
            gameObject.GetComponentInChildren<ParticleSystem>().Simulate(Time.unscaledDeltaTime, true, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("trigger exit");
        //gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}
