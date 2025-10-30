using UnityEngine;

public class FoilAttack : MonoBehaviour
{
    public Fencer fencer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Torso"))
        {
            EventManager.TriggerRoundEnd(fencer.fencerId);
        }
    }
}
