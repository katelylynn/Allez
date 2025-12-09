using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;

public class LowStaminaVolume : MonoBehaviour
{
    private PlayerStamina stamina;
    private Volume lowStaminaVolume;
    private Vignette vignette;
    private float lowVignetteVal = 0.3f;
    private float highVignetteVal = 0.5f;
    private float speed = 0.55f;
    private float resetSpeed = 0.55f;
    private int lowStamina = 20;
    private float target;
    private Coroutine resetRoutine = null;

    private void Awake()
    {
        lowStaminaVolume = GetComponent<Volume>();
        stamina = GetComponentInParent<PlayerStamina>();
        lowStaminaVolume.profile.TryGet(out vignette);
        target = highVignetteVal;
    }

    // Update is called once per frame
    void Update()
    {
        if (stamina.currentStamina < lowStamina)
        {
            try{
                StopCoroutine(resetRoutine);
            } catch (NullReferenceException) { }
            resetRoutine = null;
            showEffect();
        }
        else if (stamina.currentStamina > lowStamina)
        {
            resetEffect();
        }
    }

    private void showEffect()
    {
        vignette.smoothness.value =
            Mathf.MoveTowards(vignette.smoothness.value, target, speed * Time.deltaTime);

        if (Mathf.Abs(vignette.smoothness.value - target) < 0.001f)
        {
            target = (target == highVignetteVal) ? lowVignetteVal: highVignetteVal;
        }
    }

    private void resetEffect()
    {
        // Stop the pulsing from continuing
        target = highVignetteVal;

        // Start reset coroutine safely
        if (resetRoutine == null)
            resetRoutine = StartCoroutine(ResetVignetteRoutine());
    }

    private IEnumerator ResetVignetteRoutine()
    {
        float resetTarget = 0.01f;
        // Smoothly move back to default
        while (Mathf.Abs(vignette.smoothness.value - resetTarget) > 0.001f)
        {
            vignette.smoothness.value = Mathf.MoveTowards(
                vignette.smoothness.value,
                resetTarget,
                resetSpeed * Time.deltaTime
            );

            yield return null;
        }

        vignette.smoothness.value = resetTarget;

        resetRoutine = null;
    }

}
