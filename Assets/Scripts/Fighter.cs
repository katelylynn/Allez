using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    public void OnAttack(InputValue value)
    {
        EventManager.TriggerRoundEnd(0); // TEMP
    }

    public void OnTilt(InputValue value)
    {
        float axis = value.Get<float>();
        Debug.Log($"Tilt axis value: {axis}");
    }
}
