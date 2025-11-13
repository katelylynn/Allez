using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int pointsToWin = 3;

    private Canvas uiScore;
    private RoundStartCountDown countdownTimer;
    public string resultsScene = "resultsScene";
    public float hitTimeScale = 0.01f;
    PlayerDataManager dM;
    private bool roundSequenceRunning;
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
        dM = PlayerDataManager.GetInstance();
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
        EventManager.TriggerInputEnable(false);
        yield return StartCoroutine(countdownTimer.Run());
        //Debug.Log("Countdown finished");
        EventManager.TriggerInputEnable(true);
        EventManager.TriggerRoundStart();
    }
    private void EndRound(FencerId winner)
    {
        if (!roundSequenceRunning)
        {
            StartCoroutine(EndRoundSequence(winner));
        }
    }

    private IEnumerator EndRoundSequence(FencerId winner)
    {
        roundSequenceRunning = true;
        Time.timeScale = hitTimeScale;
        EventManager.TriggerInputEnable(false);

        /* Update score */
        int[] s = LoadScore();
        s[(int)winner]++;
        SetCurrentScore(winner, s[(int)winner]);

        //Debug.Log("hit scored! winner: fencer " + winner);
        //Debug.Log("round: " + PlayerPrefs.GetInt("CurrentRound") + ", score: " + s[0] + ", " + s[1]);

        /* Update UI */
        countdownTimer.DisplayWinner((int)winner);
        foreach (var ui in uiScore.GetComponentsInChildren<UIScoreManager>(true))
        {
            ui.Initialize(gameObject);
            ui.UpdateUI();
        }
        yield return new WaitForSecondsRealtime(2.5f);
        countdownTimer.HideWinner();

        EventManager.TriggerRoundReset();
        yield return new WaitForEndOfFrame();

        /* Check for game over */
        if (s[0] == pointsToWin || s[1] == pointsToWin)
        {
            dM.UpdatePlayerDataAfterGame(dM.p1, (int)winner == 0, s[0], PlayerPrefs.GetInt("CurrentRound"));
            dM.UpdatePlayerDataAfterGame(dM.p2, (int)winner == 1, s[1], PlayerPrefs.GetInt("CurrentRound"));
            dM.SaveData();
            Time.timeScale = 1f;
            EndFight(winner);
            roundSequenceRunning = false;
            yield break;
        }
        else
        {
            IncrementCurrentRound();
            Time.timeScale = 1f;
            StartRound();
            yield return new WaitForSecondsRealtime(0.5f);
            yield return StartCoroutine(Countdown());
            roundSequenceRunning = false;
        }
    }

    private void EndFight(FencerId winner)
    {
        Debug.Log("GAME OVER!");
        Debug.Log("winner: fencer " + (int)winner);

        /* Change to results scene */
        PlayerPrefs.SetString("RoundWinner", (int)winner == 0 ? "Player One" : "Player Two");
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
