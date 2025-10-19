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
    }
}
