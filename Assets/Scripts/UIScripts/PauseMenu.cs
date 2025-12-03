using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void TriggerUnpause()
    {
        EventManager.TriggerPause();
    }
}
