using UnityEngine;
using TMPro;

public class DisplayGameResults : MonoBehaviour
{
    TMP_Text[] resultsText;
    PlayerDataManager dM;
    void Start()
    {
        dM = PlayerDataManager.GetInstance();
        resultsText = gameObject.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text text in resultsText)
        {
            if (text.name == "WinText")
            {
                if(PlayerPrefs.GetString("RoundWinner").Equals("Player One"))
                {
                    text.text = dM.p1 +" wins in " + PlayerPrefs.GetInt("CurrentRound") + " rounds!";
                    text.color = Color.blue;
                }else
                {
                    text.text = dM.p2 + " wins in " + PlayerPrefs.GetInt("CurrentRound") + " rounds!";
                    text.color = Color.red;
                }
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
                    } else if (text2.name == "P1")
                    {
                        text2.text = dM.p1;
                    } else if (text2.name == "P2")
                    {
                        text2.text = dM.p2;
                    }
                }
            }
            else if(text.name == "PlayerStats")
            {
                PlayerDataManager.PlayerData playerData;
                if (PlayerPrefs.GetString("RoundWinner").Equals("Player One"))
                {
                    playerData = dM.gameData.data[dM.p1];
                }
                else
                {
                    playerData = dM.gameData.data[dM.p2];
                }
                    text.text = $"{playerData.name} stats:\n" +
                        $"Rounds won {playerData.roundsWon}\n" +
                        $"Rounds played {playerData.roundsPlayed}\n" +
                        $"Games won {playerData.gamesWon}\n" +
                        $"Games played {playerData.gamesPlayed}";
            }
        }       
    }
}
