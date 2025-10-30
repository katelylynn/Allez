using UnityEngine;
using TMPro;

public class DisplayGameResults : MonoBehaviour
{
    TMP_Text[] resultsText;

    void Start()
    {
        resultsText = gameObject.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text text in resultsText)
        {
            if (text.name == "WinText")
            {
                text.text = PlayerPrefs.GetString("RoundWinner") +" wins in " + PlayerPrefs.GetInt("CurrentRound") + " rounds!";
            }
            else if (text.name == "ScoreLine")
            {
                TMP_Text[] scores = text.GetComponentsInChildren<TMP_Text>();

                foreach (TMP_Text text2 in scores)
                {
                    if (text2.name == "P1Score")
                    {
                        text2.text = GameManager.LoadScore()[0].ToString();
                    }
                    else if(text2.name == "P2Score")
                    {
                        text2.text = GameManager.LoadScore()[1].ToString();
                    }
                }
            }
        }       
    }
}
