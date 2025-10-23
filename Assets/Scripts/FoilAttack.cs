using UnityEngine;

public class FoilAttack : MonoBehaviour
{
    public Fencer fencer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Torso"))
        {
            // successfully hitting your opponent's torso ends the round
            EventManager.TriggerRoundEnd(fencer.fencerId);
        }
    }
}
