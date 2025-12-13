using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerDataManager;

public class DisplayGameResults : MonoBehaviour
{
    TMP_Text[] resultsText;
    PlayerDataManager dM;
    public GameObject fencerPrefab;
    public GameObject fencerComponent1;
    public GameObject fencerComponent2;
    public GameObject defeatedFencer1;
    public GameObject defeatedFencer2;
    public TMP_Text winText;
    public TMP_Text winnerName;
    public TMP_Text winnerStats;
    public TMP_Text loserName;
    public TMP_Text loserStats;
    public TMP_Text p1Name;
    public TMP_Text p1Score;
    public TMP_Text p2Name;
    public TMP_Text p2Score;

    private float drawScaleFactor = 0.75f;

    private int winner;
    private int rounds;
    private bool vsAI;
    private bool isAIWinner;
    private bool isDraw;

    private Vector2 shrunkWinnerPosition = new Vector2(-720, 250);
    void Start()
    {
        Time.timeScale = 1f; //just incase there is a hit last second in blitz mode
        dM = PlayerDataManager.GetInstance();

        resultsText = gameObject.GetComponentsInChildren<TMP_Text>();

        winner = PlayerPrefs.GetInt("RoundWinner", -1);
        rounds = PlayerPrefs.GetInt("CurrentRound");
        vsAI = PlayerPrefs.GetString("OpponentType", null) == "AI";
        isAIWinner = vsAI && winner == 1;
        isDraw = winner == -1;
        
        SetWinText();
        SetFencers();
        SetScoreLine();
        SetWinnerStats();
        if (!vsAI)
            SetLoserStats();
        else
            HideLoserStats();
    }

    private void TestDataAIWins()
    {
        Debug.Log($"AIWINS isDraw:{isDraw}");
        dM.p1 = "FencerFinn";
        dM.p2 = "AI";
        PlayerPrefs.SetInt("RoundWinner", 1);
        PlayerPrefs.SetInt("CurrentRound", 5);
        PlayerPrefs.SetInt("P1Score", 2);
        PlayerPrefs.SetInt("P2Score", 3);
        PlayerPrefs.SetString("Player One", "FencerFinn");
        PlayerPrefs.SetString("OpponentType", "AI");
    }
    private void TestDataAIDraw()
    {
        dM.p1 = "FencerFinn";
        dM.p2 = "AI";
        PlayerPrefs.SetInt("RoundWinner", -1);
        PlayerPrefs.SetInt("CurrentRound", 4);
        PlayerPrefs.SetInt("P1Score", 2);
        PlayerPrefs.SetInt("P2Score", 2);
        PlayerPrefs.SetString("Player One", "FencerFinn");
        PlayerPrefs.SetString("OpponentType", "AI");
    }
    private void TestDataPVPDraw()
    {
        dM.p1 = "FencerFinn";
        dM.p2 = "ThrustMaster";
        PlayerPrefs.SetInt("RoundWinner", -1);
        PlayerPrefs.SetInt("CurrentRound", 4);
        PlayerPrefs.SetInt("P1Score", 2);
        PlayerPrefs.SetInt("P2Score", 2);
        PlayerPrefs.SetString("OpponentType", "Player");
    }
    private void TestDataPVPP1Win()
    {
        dM.p1 = "FencerFinn";
        dM.p2 = "ThrustMaster";
        PlayerPrefs.SetInt("RoundWinner", 0);
        PlayerPrefs.SetInt("CurrentRound", 5);
        PlayerPrefs.SetInt("P1Score", 3);
        PlayerPrefs.SetInt("P2Score", 2);
        PlayerPrefs.SetString("OpponentType", "Player");
    }
    private void TestDataPVPP2Win()
    {
        dM.p1 = "FencerFinn";
        dM.p2 = "ThrustMaster";
        PlayerPrefs.SetInt("RoundWinner", 1);
        PlayerPrefs.SetInt("CurrentRound", 5);
        PlayerPrefs.SetInt("P1Score", 2);
        PlayerPrefs.SetInt("P2Score", 3);
        PlayerPrefs.SetString("OpponentType", "Player");
    }
    private void SetFencers()
    {
        if (isDraw)
        {
            fencerComponent1.SetActive(false);
            fencerComponent2.SetActive(false);
            defeatedFencer1.SetActive(true);
            defeatedFencer2.SetActive(true);
            defeatedFencer2.GetComponent<Fencer>()
                .ChangeOutfitColor(0, GlobalColours.FencerBlue);
        }
        else if (winner == 0)
        {
            fencerPrefab.GetComponent<Fencer>()
                .ChangeOutfitColor(0, GlobalColours.FencerBlue);
            defeatedFencer1.GetComponent<Fencer>()
                .ChangeOutfitColor(0, GlobalColours.FencerRed);
        }
        else
        {
            winText.text = dM.p2 + " wins in " + rounds + " rounds!";
            defeatedFencer1.GetComponent<Fencer>()
                .ChangeOutfitColor(0, GlobalColours.FencerBlue);
        }
    }
    private void SetWinText()
    {
        winText.color = GlobalColours.White;
        // TIE
        if (isDraw)
        {
            winText.text = $"It's a tie after {rounds} rounds!";
            winText.color = Color.yellow;
        }
        // PLAYER ONE WINS
        else if (winner == 0)
        {
            winText.text = dM.p1 + " wins in " + rounds + " rounds!";
        }
        // PLAYER TWO WINS
        else
        {
            winText.text = dM.p2 + " wins in " + rounds + " rounds!";
        }
    }

    private void SetScoreLine()
    {
        p1Score.text = GameManager.LoadScore()[0].ToString();
        p2Score.text = GameManager.LoadScore()[1].ToString();
        p1Name.text = dM.p1;
        p1Name.color = GlobalColours.Blue;
        p2Name.text = dM.p2;
        p2Name.color = GlobalColours.Red;
    }

    private void SetWinnerStats()
    {
        if (isDraw) //shrink winner size if draw
        {
            SetDrawStats();
        }
        else
        {
            PlayerDataManager.PlayerData playerData;

            if (winner == 0 || winner == -1)
            {
                winnerName.color = GlobalColours.Blue;
                loserName.color = GlobalColours.Red;
            }
            else
            {
                winnerName.color = GlobalColours.Red;
                loserName.color = GlobalColours.Blue;
            }

            if (winner == 0 || vsAI && winner == 1)
            {
                playerData = dM.gameData.data[dM.p1];
                winnerName.color = GlobalColours.Blue;
            }
            else
            {
                playerData = dM.gameData.data[dM.p2];
            }

            winnerName.text = playerData.name;
            winnerStats.text = $"Rounds won {playerData.roundsWon}\n" +
                        $"Rounds played {playerData.roundsPlayed}\n" +
                        $"Games won {playerData.gamesWon}\n" +
                        $"Games played {playerData.gamesPlayed}";
        }
    }

    private void SetLoserStats()
    {
        PlayerDataManager.PlayerData playerData;
        int loser = winner == 1 ? 0 : 1;
        if (loser == 0)
        {
            playerData = dM.gameData.data[dM.p1];
        }
        else
        {
            playerData = dM.gameData.data[dM.p2];
        }

        loserName.text = playerData.name;

        loserStats.text = $"Rounds won {playerData.roundsWon}\n" +
                    $"Rounds played {playerData.roundsPlayed}\n" +
                    $"Games won {playerData.gamesWon}\n" +
                    $"Games played {playerData.gamesPlayed}";

    }
    void HideLoserStats()
    {
        loserStats.enabled = false;
        loserStats.transform.parent.gameObject.SetActive(false);
    }
    void SetDrawStats()
    {
        PlayerDataManager.PlayerData playerData;

        //resize and move winner panel to match loser panel size and pos
        winnerStats.transform.parent.transform.localScale = new Vector3(drawScaleFactor, drawScaleFactor, drawScaleFactor);
        winnerStats.transform.parent.GetComponent<RectTransform>().anchoredPosition = shrunkWinnerPosition;

        playerData = dM.gameData.data[dM.p1];
        winnerName.text = playerData.name;
        winnerName.color = GlobalColours.Blue;
        winnerStats.text = $"Rounds won {playerData.roundsWon}\n" +
                        $"Rounds played {playerData.roundsPlayed}\n" +
                        $"Games won {playerData.gamesWon}\n" +
                        $"Games played {playerData.gamesPlayed}";

        if (vsAI)
        {
            HideLoserStats();
        }
        else
        {
            playerData = dM.gameData.data[dM.p2];
            loserName.text = playerData.name;
            loserName.color = GlobalColours.Red;
            loserStats.text = $"Rounds won {playerData.roundsWon}\n" +
                            $"Rounds played {playerData.roundsPlayed}\n" +
                            $"Games won {playerData.gamesWon}\n" +
                            $"Games played {playerData.gamesPlayed}";
        }
    }
}
