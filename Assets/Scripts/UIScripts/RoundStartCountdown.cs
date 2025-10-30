using System.Collections;
using TMPro;
using UnityEngine;

public class RoundStartCountDown : MonoBehaviour
{
    public float countdownTickDuration = 1.5f;
    public Transform textParent;
    private TMP_Text[] countdownTexts;
    private float countdownTime = 3f;
    public TMP_Text roundWinner;

    public IEnumerator Run()
    {
        countdownTexts = textParent.GetComponentsInChildren<TMP_Text>(true);

        yield return StartCoroutine(Countdown());
    }

    public IEnumerator Countdown()
    {
        float count = countdownTime;
        
        while (count > 0)
        {
            int i = Mathf.RoundToInt(countdownTime - count);

            foreach (var t in countdownTexts)
                t.gameObject.SetActive(false);

            if (i >= 0 && i < countdownTexts.Length)
                countdownTexts[i].gameObject.SetActive(true);

            yield return new WaitForSeconds(countdownTickDuration);
            count--;
        }

        foreach (var t in countdownTexts)
            t.gameObject.SetActive(false);
    }

    public void DisplayWinner(int winner)
    {
        string winnerText;

        if (winner == 0)
        {
            winnerText = "Player one";
            roundWinner.color = Color.blue;
        }
        else
        {
            winnerText = "Player two";
            roundWinner.color = Color.red;
        }
        winnerText += " scores a touch!";

        roundWinner.text = winnerText;  
        roundWinner.gameObject.SetActive(true);
    }

    public void HideWinner()
    {
        roundWinner.gameObject.SetActive(false);
    }
}
