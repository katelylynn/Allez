using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int pointsToWin = 3;

    private Canvas uiScore;
    private RoundStartCountDown countdownTimer;

    public string resultsScene = "resultsScene";

    public void SetUIScore(Canvas ui)
    {
        uiScore = ui;
    }

    public void SetCountdownTimer(RoundStartCountDown timer)
    {
        countdownTimer = timer;
    }

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

    public void StartRound()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        yield return StartCoroutine(countdownTimer.Run());
        Debug.Log("Countdown finished");
        EventManager.TriggerRoundStart();
    }

    private void EndRound(FencerId winner)
    {
        /* Update score */
        int[] s = LoadScore();
        s[(int)winner]++;
        SetCurrentScore(winner, s[(int)winner]);

        Debug.Log("hit scored! winner: fencer " + winner);
        Debug.Log("round: " + PlayerPrefs.GetInt("CurrentRound") + ", score: " + s[0] + ", " + s[1]);

        /* Update UI */
        StartCoroutine(DisplayRoundWinner(winner));

        foreach (var ui in uiScore.GetComponentsInChildren<UIScoreManager>(true))
        {
            ui.Initialize(gameObject);
            ui.UpdateUI();              
        }

        /* Check for game over */
        if (s[0] == pointsToWin || s[1] == pointsToWin)
        {
            EndFight(winner);
        }
        else
        {
            IncrementCurrentRound();
            StartRound();
        }
    }

    private void EndFight(FencerId winner)
    {
        Debug.Log("GAME OVER!");
        Debug.Log("winner: fencer " + (int)winner);

        /* Change to results scene */
        PlayerPrefs.SetString("RoundWinner", (int)winner == 0 ? "Player One": "Player Two");
        SceneSwapper.ChangeScene(resultsScene);
    }

    public static int[] LoadScore()
    {
        return new int[2] { PlayerPrefs.GetInt("P1Score"), PlayerPrefs.GetInt("P2Score") };
    }

    private void SetCurrentScore(FencerId fencerId, int num)
    {
        if (fencerId == FencerId.Fencer0)
            PlayerPrefs.SetInt("P1Score", num);
        else
            PlayerPrefs.SetInt("P2Score", num);
    }

    IEnumerator DisplayRoundWinner(FencerId winner)
    {
        //Time.timeScale = 0;
        countdownTimer.DisplayWinner((int)winner);

        yield return new WaitForSecondsRealtime(2.0f);

        countdownTimer.HideWinner();
        //Time.timeScale = 1;
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
