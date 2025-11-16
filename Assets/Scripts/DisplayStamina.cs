using TMPro;
using UnityEngine;

public class DisplayStamina : MonoBehaviour
{
    public PlayerStamina stamina;
    public TMP_Text tmp_text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        stamina = GetComponentInParent<PlayerStamina>();
        tmp_text = GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        tmp_text.text = $"Stamina: {stamina.currentStamina}";
    }
}
