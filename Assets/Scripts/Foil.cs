using System.Collections;
using UnityEngine;

public class Foil : MonoBehaviour
{
    public Fencer fencer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Foil"))
        {
            EventManager.TriggerParrySuccess();
        }
        else if (other.CompareTag("Torso"))
        {
            // successfully hitting your opponent's torso ends the round
            EventManager.TriggerRoundEnd(fencer.fencerId);
        }
    }
}
