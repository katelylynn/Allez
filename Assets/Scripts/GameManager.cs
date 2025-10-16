using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int pointsToWin = 3;
    public Canvas uiScore; //leave this unassigned in the editor
    public RoundStartCountDown countdownTimer;
    private void Awake()
    {
        ResetGameState();
    }
    private void Start()
    {
        EventManager.RoundEnd += EndRound;
    }
    private void OnDestroy()
    {
        EventManager.RoundEnd -= EndRound;
    }
    public void StartDuel()
    {
        Countdown();
        EventManager.TriggerRoundStart();
    }

    private void Countdown()
    {
        countdownTimer.Run();
        Debug.Log("En garde, pret, allez!");
    }

    private void EndRound(FencerId winner)
    {
        int[] s = LoadScore();
        int currRound = PlayerPrefs.GetInt("CurrentRound");
        s[(int)winner]++;
        SetCurrentScore((int)winner, s[(int)winner]);

        Debug.Log("hit scored! winner: fencer " + winner);
        Debug.Log("round: " + currRound + ", score: " + s[0] + ", " + s[1]);

        //refresh both player UIs
        foreach (var ui in uiScore.GetComponentsInChildren<UIScoreManager>(true))
        {
            ui.Initialize(gameObject);
            ui.UpdateUI();              
        }

        if (s[0] == pointsToWin || (s[1] == pointsToWin))
        {
            EndFight(winner);
        }
        else
        {
            IncrementCurrentRound();
            StartDuel();
        }
    }

    private void EndFight(FencerId winner)
    {
        Debug.Log("GAME OVER!");
        Debug.Log("winner: fencer " + (int)winner);
        PlayerPrefs.SetString("RoundWinner", (int)winner == 0 ? "Player One": "Player Two");
        SceneSwapper.ChangeScene("ResultsScene");
    }

    public static int[] LoadScore()
    {
        return new int[2] { PlayerPrefs.GetInt("P1Score"), PlayerPrefs.GetInt("P2Score") };
    }

    private void SetCurrentScore(int playerID, int num)
    {
        if (playerID == 0)
        {
            PlayerPrefs.SetInt("P1Score", num);
        }
        else
        {
            PlayerPrefs.SetInt("P2Score", num);
        }
    }

    private void IncrementCurrentRound()
    {
        PlayerPrefs.SetInt("CurrentRound", (PlayerPrefs.GetInt("CurrentRound") + 1));
    }

    private void ResetGameState()
    {
        PlayerPrefs.SetInt("P1Score", 0);
        PlayerPrefs.SetInt("P2Score", 0);
        PlayerPrefs.SetInt("CurrentRound", 1);
    }
}
