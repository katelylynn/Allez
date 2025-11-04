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
            Debug.Log($"⚔️ Clash at position: {contact}");
            Destroy(effect.gameObject, effect.main.duration);

            // Trigger parry success event
            EventManager.TriggerParrySuccess();
        }
    }
}