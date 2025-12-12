/*
    Foil Attack
    Handles behavior for the tip of the blade, where you can hit your opponent.
*/

using UnityEngine;

public class FoilAttack : MonoBehaviour
{
    public Fencer fencer;
    private static bool hitRegistered = false;
    private ParticleSystem ps;
    
    private void Awake() => ps = GetComponentInChildren<ParticleSystem>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Torso")) return;

        // if someone already scored, ignore further hits
        if (hitRegistered) return;

        hitRegistered = true;

        EventManager.TriggerRoundEnd(fencer.fencerId);
        ps.Play();
        ps.Simulate(Time.unscaledDeltaTime, true, false);
    }

    public static void ResetHit() => hitRegistered = false;
}
