using System.Collections;
using UnityEngine;

public class Foil : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Foil"))
        {
            EventManager.TriggerParrySuccess();
        }
    }
}
