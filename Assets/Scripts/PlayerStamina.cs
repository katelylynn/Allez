using System.Collections;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina System Values")]
    public int currentStamina = 100;
    public int maxStamina = 100;
    [Tooltip("How fast stamina bar fills")]
    public int staminaRegenRate = 1;
    [Tooltip("How long until stamina starts to regen in seconds")]
    public float staminaRegenDelay = 2f;
    float currentStaminaRechargeTime = 2f;
    bool isStaminaRecharging = false;

    void Awake()
    {
        StartCoroutine(StaminaRechargeDelayRoutine());
    }
    private void OnDisable()
    {
        isStaminaRecharging = false;
        currentStamina = maxStamina;
        StopCoroutine(RechargeStaminaRoutine());
        StopCoroutine(StaminaRechargeDelayRoutine());
    }
    void FixedUpdate()
    {

    }

    public bool ConsumeStamina(int stamina)
    {
        if (currentStamina < 0) return false;
        isStaminaRecharging = false;
        currentStamina -= stamina;
        StopAllCoroutines();
        ResetRechargeDelay();
        StartCoroutine(StaminaRechargeDelayRoutine());
        return true;
    }

    IEnumerator StaminaRechargeDelayRoutine()
    {
        while (currentStamina < maxStamina)
        {
            if (!isStaminaRecharging && currentStamina < maxStamina)
            {
                //Debug.Log($"staminadelay before : {currentStaminaRechargeTime}");
                currentStaminaRechargeTime -= Time.deltaTime;
                //Debug.Log($"staminadelay after : {currentStaminaRechargeTime}");

                if (currentStaminaRechargeTime <= 0)
                    StartCoroutine(RechargeStaminaRoutine());
            }
            yield return new WaitForFixedUpdate();
        }
    }

    void ResetRechargeDelay()
    {
        currentStaminaRechargeTime = staminaRegenDelay;
    }

    IEnumerator RechargeStaminaRoutine()
    {
        while (currentStamina < maxStamina)
        {
            isStaminaRecharging = true;
            currentStamina = (currentStamina + staminaRegenRate > maxStamina) ? maxStamina : currentStamina + staminaRegenRate;
            yield return new WaitForSeconds(0.1f);
        }
        isStaminaRecharging = false;
    }

    public void AddStamina(int value)
    {
        currentStamina = currentStamina + value > 100 ? 100: currentStamina + value;

    }

    public void ConsumeStaminaWhenParried(int stamina)
    {
        currentStamina -= stamina;
        isStaminaRecharging = false;
    }
}
