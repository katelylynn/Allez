using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneInitializer : MonoBehaviour
{
    public GameMode gameMode;

    public GameObject gameManagerPrefab;
    public GameObject fencerPrefab; 
    public GameObject combatManagerPrefab;
    public GameObject environmentPrefab;
    public GameObject scoreUIPrefab;
    public GameObject countdownUIPrefab;
    public GameObject staminaUIPrefab;
    public GameObject pauseUIPrefab;
    // public GameObject eventSystemPrefab;

    private GameObject g;

    public FencerType fencer0Type; 
    public FencerType fencer1Type;

    public float skyboxRotation = 0;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 1f / 60f;
        SpawnPrefabs();
        g.GetComponent<GameManager>().StartBout();
    }

    private void SpawnPrefabs()
    {
        /* FENCERS */
        GameObject f0 = Spawn(fencerPrefab); // FEMALE
        f0.GetComponent<Fencer>().Initialize(FencerId.Fencer0, fencer0Type);

        GameObject f1 = Spawn(fencerPrefab); // MALE

        // Set the correct opponent type
        string opponentType = PlayerPrefs.GetString("OpponentType", null);

        if (opponentType == "Player")
            fencer1Type = FencerType.Player;
        else if (opponentType == "AI")
            fencer1Type = FencerType.AI;

        f1.GetComponent<Fencer>().Initialize(FencerId.Fencer1, fencer1Type);

        // If the opponent is AI
        if (fencer1Type == FencerType.AI) {
            f1.GetComponent<AI>().enabled = true;
            f1.GetComponent<AI>().Initialize(f0);
        }

        // Set up 2 different audios
        f0.GetComponent<PlayerAudioController>().SetGenderAudios(FencerId.Fencer0);
        f0.GetComponent<PlayerAudioController>().SetGenderAudios(FencerId.Fencer1);


        // Set opponent's torso as the aim target for both players
        f0.GetComponent<Fencer>().SetAimTarget(f1.GetComponent<Fencer>().aimTarget);
        f1.GetComponent<Fencer>().SetAimTarget(f0.GetComponent<Fencer>().aimTarget);

        // Change fencer 1's outfit color
        f0.GetComponent<S_A_SkinnedOutfitColorChange>().ChangeOutfitColor(0, GlobalColours.FencerBlue);

        /* MANAGERS */
        g = Spawn(gameManagerPrefab);

        string gm = PlayerPrefs.GetString("GameMode", null);

        if (gm == "First to X Points")
            gameMode = GameMode.FirstToX;
        else if (gm == "Most Points in X Seconds")
            gameMode = GameMode.MostPointsInXTime;


        GameObject cm = Spawn(combatManagerPrefab);
        cm.GetComponent<CombatManager>().Initialize(f0.GetComponent<Fencer>(), f1.GetComponent<Fencer>());

        /* UI */
        GameObject countdownUI = Spawn(countdownUIPrefab);
        g.GetComponent<GameManager>().SetCountdownTimer(countdownUI.GetComponentInChildren<RoundStartCountDown>());

        GameObject scoreUI = Spawn(scoreUIPrefab);
        g.GetComponent<GameManager>().SetUIScore(scoreUI.GetComponent<Canvas>());


        GameObject staminaUI = Spawn(staminaUIPrefab);
        staminaUI.GetComponent<StaminaBarManager>().Initialize(f0, f1);
        g.GetComponent<GameManager>().SetStaminaUI(staminaUI.GetComponent<Canvas>());

        // Setup both players UI managers
        UIScoreManager[] uiManagers = scoreUI.GetComponentsInChildren<UIScoreManager>(true);

        GameObject pauseUI = Spawn(pauseUIPrefab);
        pauseUI.SetActive(false);
        // Spawn(eventSystemPrefab);

        // Initialize game manager
        g.GetComponent<GameManager>().Initialize(gameMode, PlayerPrefs.GetInt("PointsToWin", -1), PlayerPrefs.GetInt("BoutLength", -1), pauseUI);

        foreach (var ui in uiManagers)
        {
            ui.Initialize(g);
            ui.UpdateUI();
        }

        /* ENVIRONMENT */
        Spawn(environmentPrefab);

        
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", skyboxRotation);
    }

    private GameObject Spawn(GameObject prefab)
    {
        GameObject go = Instantiate(prefab);
        go.name = prefab.name;
        return go;
    }
}
