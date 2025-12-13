/*
    Foil
    Handles behavior for collision with the edge of the blade, not the tip.
*/

using UnityEngine;

public class Foil : MonoBehaviour
{
    public ParticleSystem clashEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Foil"))
        {
            // Play clash particle
            Vector3 contact = other.ClosestPoint(transform.position);
            ParticleSystem effect = Instantiate(clashEffect, contact, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration);

            // Trigger parry success event
            EventManager.TriggerParrySuccess();
        }
    }
}