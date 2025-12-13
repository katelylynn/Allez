/*
    Game Manager
    Handles the core gameloop, such as point and time tracking.
*/

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;

public enum GameMode
{
    FirstToX,
    MostPointsInXTime,
}

public class GameManager : MonoBehaviour
{
    public GameMode gameMode;

    // first to X mode
    public int pointsToWin = 3;

    // most points in X time mode
    public float maxTime = 60f; // seconds
    public int elapsedTime = 0;
    public bool countdownRunning = false;

    // UI and data
    Canvas uiScore;
    Canvas staminaUI;
    [SerializeField] private GameObject pauseUI;
    private RoundStartCountDown countdownTimer;
    public string resultsScene = "resultsScene";
    public float hitTimeScale = 0.01f;
    PlayerDataManager dM;
    private bool roundSequenceRunning;

    // sound
    [Header("Time Milestone SFX")]
    public AudioSource sfxSource;      
    public AudioClip TimerWhistle;
    private bool played30;
    private bool played60;
    private bool played90;
    
    // game management
    public bool IsRoundBusy => countdownRunning || roundSequenceRunning;
    public static GameManager Instance { get; private set; }
    public bool isGameActive = false;

    private void Awake()
    {
        // game singleton
        Instance = this;        
        dM = PlayerDataManager.GetInstance();
        ResetGameState();

        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
    }

    public void Initialize(GameMode gm, int ptw, int bl, GameObject pui)
    {
        gameMode = gm;
        pauseUI = pui;

        // manages points/time based on which game mode was chosen
        if (ptw != -1) pointsToWin = ptw;
        if (bl != -1) maxTime = (float) bl;
        if (gameMode == GameMode.MostPointsInXTime) pointsToWin = 0;
    }

    // setters for UI variables
    public void SetUIScore(Canvas ui) => uiScore = ui;
    public void SetStaminaUI(Canvas ui) => staminaUI = ui;
    public void SetCountdownTimer(RoundStartCountDown timer) => countdownTimer = timer;

    private void Start()
    {
        Time.timeScale = 1.0f;
        EventManager.RoundEnd += EndRound;
        EventManager.Pause += Pause;
    }

    public void StartBout()
    {
        elapsedTime = 0;
        played30 = played60 = played90 = false;

        StartRound();

        if (gameMode == GameMode.MostPointsInXTime) 
        {
            uiScore.transform.Find("CountdownText").GetComponent<TMP_Text>().text =
                (maxTime - elapsedTime) + "s";
            StartCoroutine(BoutCountdown());
        }
        else if (gameMode == GameMode.FirstToX)
        {
            uiScore.transform.Find("CountdownText").GetComponent<TMP_Text>().text = "";
        }
    }

    // countdown for the "most points in X seconds" mode
    private IEnumerator BoutCountdown()
    {
        // Debug.Log("Bout timer started!");
        float secondAccumulator = 0f;

        while (elapsedTime < maxTime)
        {
            uiScore.transform.Find("CountdownText").GetComponent<TMP_Text>().text =
                (maxTime - elapsedTime) + "s";

            // Only count time when not in countdown / round sequence
            if (!countdownRunning && !roundSequenceRunning)
            {
                secondAccumulator += Time.deltaTime;

                if (secondAccumulator >= 1f)
                {
                    secondAccumulator -= 1f;
                    elapsedTime++;
                    // Debug.Log("Tick: " + elapsedTime);
                    CheckTimeMilestones();
                }
            }

            yield return null;
        }

        // Debug.Log("Time is up! " + maxTime + " seconds have passed.");
        EndFight(DetermineWinner());
    }

    private void CheckTimeMilestones()
    {
        if (!played30 && elapsedTime >= 30)
        {
            PlayMilestoneSFX();
            played30 = true;
        }

        if (!played60 && elapsedTime >= 60)
        {
            PlayMilestoneSFX();
            played60 = true;
        }

        if (!played90 && elapsedTime >= 90)
        {
            PlayMilestoneSFX();
            played90 = true;
        }
    }

    // plays audio queue on certain time milestones
    private void PlayMilestoneSFX()
    {
        if (TimerWhistle == null) return;
        if (sfxSource == null) return;

        sfxSource.PlayOneShot(TimerWhistle);
    }

    public void StartRound() => StartCoroutine(RoundCountdown());

    public void Pause()
    {
        Time.timeScale = 1.0f - Time.timeScale; 
        pauseUI.SetActive(!pauseUI.activeSelf); 
    }

    private FencerId DetermineWinner()
    {
        int[] score = LoadScore();
        if (score[0] > score[1])
            return FencerId.Fencer0;
        if (score[0] < score[1])
            return FencerId.Fencer1;
        return FencerId.None;
    }


    private IEnumerator RoundCountdown()
    {
        countdownRunning = true;
        isGameActive = false;
        EventManager.TriggerInputEnable(false);
        yield return StartCoroutine(countdownTimer.Run());
        EventManager.TriggerInputEnable(true);
        countdownRunning = false;
        EventManager.TriggerRoundStart();
        isGameActive = true;
        FoilAttack.ResetHit();
    }

    private void EndRound(FencerId winner)
    {
        if (!roundSequenceRunning)
            StartCoroutine(EndRoundSequence(winner));
    }

    private IEnumerator EndRoundSequence(FencerId winner)
    {
        isGameActive = false;

        roundSequenceRunning = true;
        Time.timeScale = hitTimeScale;
        EventManager.TriggerInputEnable(false);

        // update score
        int[] s = LoadScore();
        s[(int)winner]++;
        SetCurrentScore(winner, s[(int)winner]);

        //Debug.Log("hit scored! winner: fencer " + winner);
        //Debug.Log("round: " + PlayerPrefs.GetInt("CurrentRound") + ", score: " + s[0] + ", " + s[1]);

        // update UI for each player
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

        // check for game over for game mode "Most points in X time"
        if (gameMode == GameMode.MostPointsInXTime) 
        {
            int[] score = LoadScore();
            pointsToWin = Math.Max(score[0], score[1]);
            foreach (var ui in uiScore.GetComponentsInChildren<UIScoreManager>(true))
            {
                ui.UpdateUI();
            }
        }

        // check for game over for game mode "First to X"
        if (gameMode == GameMode.FirstToX && (s[0] == pointsToWin || s[1] == pointsToWin))
        {
            dM.UpdatePlayerDataAfterGame(dM.p1, (int)winner == 0, s[0], PlayerPrefs.GetInt("CurrentRound"));
            if (PlayerPrefs.GetString("OpponentType", null) == "Player")
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
            staminaUI.GetComponent<StaminaBarManager>().ResetStaminaBars();
            yield return new WaitForSecondsRealtime(0.5f);
            StartRound();
            roundSequenceRunning = false;
        }
    }

    private void EndFight(FencerId winner)
    {
        isGameActive = false;

        // Debug.Log("GAME OVER!");
        // Debug.Log("winner: fencer " + (int)winner);

        // change to results scene
        PlayerPrefs.SetInt("RoundWinner", (int)winner);
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

    private void OnDestroy()
    {
        EventManager.RoundEnd -= EndRound;
        EventManager.Pause -= Pause;
    }
}
