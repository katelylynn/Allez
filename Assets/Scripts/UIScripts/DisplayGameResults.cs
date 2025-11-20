using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DisplayGameResults : MonoBehaviour
{
    TMP_Text[] resultsText;
    PlayerDataManager dM;
    public GameObject fencerPrefab;
    public GameObject fencerComponent1;
    public GameObject fencerComponent2;

    void Start()
    {
        dM = PlayerDataManager.GetInstance();
        resultsText = gameObject.GetComponentsInChildren<TMP_Text>();

        // New: read winner as an int. -1 = tie, 0 = P1, 1 = P2
        int winner = PlayerPrefs.GetInt("RoundWinner", -1);
        int rounds = PlayerPrefs.GetInt("CurrentRound");

        foreach (TMP_Text text in resultsText)
        {
            if (text.name == "WinText")
            {
                // TIE CASE
                if (winner == -1)
                {
                    text.text = $"It's a tie after {rounds} rounds!";
                    text.color = Color.yellow; // or whatever highlight color you like
                    fencerComponent1.SetActive(false);
                    fencerComponent2.SetActive(false);
                }
                // PLAYER ONE WINS
                else if (winner == 0 || PlayerPrefs.GetString("RoundWinner").Equals("Player One"))
                {
                    text.text = dM.p1 + " wins in " + rounds + " rounds!";
                    text.color = Color.blue;
                    fencerPrefab.GetComponent<S_A_SkinnedOutfitColorChange>()
                        .ChangeOutfitColor(0, new Color(0.15f, 0.24f, 0.67f));
                }
                // PLAYER TWO WINS
                else
                {
                    text.text = dM.p2 + " wins in " + rounds + " rounds!";
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
                    else if (text2.name == "P2Score")
                    {
                        text2.text = GameManager.LoadScore()[1].ToString();
                    }
                    else if (text2.name == "P1")
                    {
                        text2.text = dM.p1;
                    }
                    else if (text2.name == "P2")
                    {
                        text2.text = dM.p2;
                    }
                }
            }
            else if (text.name == "PlayerStats")
            {
                bool vsAI = PlayerPrefs.GetString("OpponentType", null) == "AI";

                // Hide stats if vs AI or tie (no single winner)
                if (vsAI || winner == -1)
                {
                    text.enabled = false;
                }
                else
                {
                    PlayerDataManager.PlayerData playerData;

                    if (winner == 0 || PlayerPrefs.GetString("RoundWinner").Equals("Player One"))
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
}
