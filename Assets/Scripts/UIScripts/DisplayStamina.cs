using TMPro;
using UnityEngine;

public class DisplayStamina : MonoBehaviour
{
    public StaminaController stamina;
    public TMP_Text tmp_text;

    void Awake()
    {
        stamina = GetComponentInParent<StaminaController>();
        tmp_text = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        tmp_text.text = $"Stamina: {stamina.currentStamina}";
    }
}
