using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarManager : MonoBehaviour
{
    PlayerStamina p1_stamina;
    PlayerStamina p2_stamina;

    public Image p1_green_bar;
    public Image p1_yellow_bar;
    public Image p2_green_bar;
    public Image p2_yellow_bar;
    public RectTransform p1_marker;
    public RectTransform p2_marker;

    float smoothGreenFillSpeed = 0.1f;
    public float yellowBarShrinkDelay = 1.0f;
    public float yellowShrinkRate = 13f;

    float currentP1ShrinkTime;
    float currentP2ShrinkTime;
    bool isP1YellowBehind = false;
    bool isP2YellowBehind = false;
    int p1oldStamina;
    int p2oldStamina;
    Coroutine p1Routine;
    Coroutine p2Routine;

    private float p1MarkerWidth;
    private float p2MarkerWidth;
    public void Initialize(GameObject p1, GameObject p2)
    {
        p1_stamina = p1.GetComponent<PlayerStamina>();
        p2_stamina = p2.GetComponent<PlayerStamina>();
        p1oldStamina = p1_stamina.currentStamina;
        p2oldStamina = p2_stamina.currentStamina;
    }
    void Awake()
    {
        currentP1ShrinkTime = yellowBarShrinkDelay;
        currentP2ShrinkTime = yellowBarShrinkDelay;
        p1MarkerWidth = p1_marker.rect.width;
        p2MarkerWidth = p2_marker.rect.width;
    }
    private void OnDisable()
    {
        p1oldStamina = p1_stamina.currentStamina;
        p2oldStamina = p2_stamina.currentStamina;

        currentP1ShrinkTime = yellowBarShrinkDelay;
        currentP2ShrinkTime = yellowBarShrinkDelay;

        isP1YellowBehind = false;
        isP2YellowBehind = false;

        p1_green_bar.fillAmount = 1f;
        p2_green_bar.fillAmount = 1f;
        p1_yellow_bar.fillAmount = 1f;
        p2_yellow_bar.fillAmount = 1f;

        if (p1Routine != null) StopCoroutine(p1Routine);
        if (p2Routine != null) StopCoroutine(p2Routine);
    }
    private void Update()
    {
        if(p1oldStamina + 20 <= p1_stamina.currentStamina || p2oldStamina + 20 <= p2_stamina.currentStamina)
        {
            BurstGreenBarFill();
        }
        GreenBarSmoothFill();

        if (p1_stamina.currentStamina < p1oldStamina)
        {
            isP1YellowBehind = true;
            if (p1Routine != null) StopCoroutine(p1Routine);
            p1Routine = StartCoroutine(P1YellowBarShrinkRoutine());
        }
        if (p2_stamina.currentStamina < p2oldStamina)
        {
            isP2YellowBehind = true;
            if (p2Routine != null) StopCoroutine(p2Routine);
            p2Routine = StartCoroutine(P2YellowBarShrinkRoutine());
        }

        if (!isP1YellowBehind)
        {
            p1_yellow_bar.fillAmount = p1_green_bar.fillAmount;
        }
        if (!isP2YellowBehind)
        {
            p2_yellow_bar.fillAmount = p2_green_bar.fillAmount;
        }
        p1oldStamina = p1_stamina.currentStamina;
        p2oldStamina = p2_stamina.currentStamina;

        float p1_fill = p1_green_bar.fillAmount;
        float p2_fill = p2_green_bar.fillAmount;
        float p1_offset = 2 * -p1MarkerWidth * (p1_fill - 0.5f);
        float p2_offset = 2 * p2MarkerWidth * (p2_fill - 0.5f);
        p1_marker.anchorMin = new Vector2(p1_fill, 0.5f);
        p1_marker.anchorMax = new Vector2(p1_fill, 0.5f);
        p1_marker.anchoredPosition = new Vector2(p1_offset, 0);
        
        p2_marker.anchorMin = new Vector2(1f - p2_fill, 0.5f);
        p2_marker.anchorMax = new Vector2(1f - p2_fill, 0.5f);
        p2_marker.anchoredPosition = new Vector2(p2_offset, 0);
        
    }

    IEnumerator P1YellowBarShrinkRoutine()
    {
        float timer = yellowBarShrinkDelay;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        while (p1_yellow_bar.fillAmount > p1_green_bar.fillAmount)
        {
            p1_yellow_bar.fillAmount = Mathf.MoveTowards(
                p1_yellow_bar.fillAmount,
                p1_green_bar.fillAmount,
                yellowShrinkRate * Time.deltaTime);
            yield return null;
        }

        isP1YellowBehind = false;
        p1Routine = null;
    }

    IEnumerator P2YellowBarShrinkRoutine()
    {
        float timer = yellowBarShrinkDelay;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        while (p2_yellow_bar.fillAmount > p2_green_bar.fillAmount)
        {
            p2_yellow_bar.fillAmount = Mathf.MoveTowards(
                p2_yellow_bar.fillAmount,
                p2_green_bar.fillAmount,
                yellowShrinkRate * Time.deltaTime);

            yield return null;
        }

        isP2YellowBehind = false;
        p2Routine = null;
    }

    void GreenBarSmoothFill()
    {
        float p1_targetFill = (float)p1_stamina.currentStamina / p1_stamina.maxStamina;
        float p2_targetFill = (float)p2_stamina.currentStamina / p2_stamina.maxStamina;

        if (p1_targetFill < p1_green_bar.fillAmount)
        {
            p1_green_bar.fillAmount = p1_targetFill;
        }
        else
        {
            p1_green_bar.fillAmount = Mathf.MoveTowards(
                p1_green_bar.fillAmount,
                p1_targetFill,
                smoothGreenFillSpeed * Time.deltaTime * p1_stamina.staminaRegenRate
            );
        }

        if (p2_targetFill < p2_green_bar.fillAmount)
        {
            p2_green_bar.fillAmount = p2_targetFill;
        }
        else
        {
            p2_green_bar.fillAmount = Mathf.MoveTowards(
                p2_green_bar.fillAmount,
                p2_targetFill,
                smoothGreenFillSpeed * Time.deltaTime * p2_stamina.staminaRegenRate
            );
        }
    }
    public void BurstGreenBarFill()
    {
        //Debug.Log("burst fill called");
        float p1_targetFill = (float)p1_stamina.currentStamina / p1_stamina.maxStamina;
        float p2_targetFill = (float)p2_stamina.currentStamina / p2_stamina.maxStamina;

        if(p1oldStamina + 20 <= p1_stamina.currentStamina)
        {
            p1_green_bar.fillAmount = p1_targetFill;
        }
        if (p2oldStamina + 20 <= p2_stamina.currentStamina)
        {
            p2_green_bar.fillAmount = p2_targetFill;
        }
    }
    public void ResetStaminaBars()
    {
        if (p1Routine != null) StopCoroutine(p1Routine);
        if (p2Routine != null) StopCoroutine(p2Routine);

        p1oldStamina = p1_stamina.currentStamina;
        p2oldStamina = p2_stamina.currentStamina;

        p1_stamina.currentStamina = p1_stamina.maxStamina;
        p2_stamina.currentStamina = p2_stamina.maxStamina;

        currentP1ShrinkTime = yellowBarShrinkDelay;
        currentP2ShrinkTime = yellowBarShrinkDelay;

        isP1YellowBehind = false;
        isP2YellowBehind = false;

        p1_green_bar.fillAmount = 1f;
        p2_green_bar.fillAmount = 1f;
        p1_yellow_bar.fillAmount = 1f;
        p2_yellow_bar.fillAmount = 1f;
    }
}
