using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
            Debug.Log("Attack pressed");
        else
            Debug.Log("Attack released");

        EventManager.TriggerRoundEnd(0); // TEMP
    }

    public void OnTilt(InputValue value)
    {
        float axis = value.Get<float>();
        Debug.Log($"Tilt axis value: {axis}");
    }
}
