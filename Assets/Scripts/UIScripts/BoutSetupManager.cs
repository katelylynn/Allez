using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoutSetupManager : MonoBehaviour
{
    PlayerDataManager dM;
    public TMP_Dropdown gameModeDropdown;
    public TMP_Dropdown opponentTypeDropdown;
    public TMP_Text pointsToWinText;
    public TMP_Dropdown pointsToWinDropdown;
    public TMP_Text boutLengthText;
    public TMP_Dropdown boutLengthDropdown;
    public GameObject selection1;
    public GameObject selection2;
    public Transform contentP1;
    public Transform contentP2;
    public TMP_Text p1HeaderText;
    public TMP_Text p2HeaderText;
    public GameObject buttonPrefab;
    public GameObject startMatchButton;
    public List<GameObject> buttonList;
    public List<GameObject> fencers;
    private bool isAI;

    // PlayerPrefs keys
    private const string KEY_OPPONENT_TYPE = "OpponentType";
    private const string KEY_GAME_MODE_INDEX = "GameModeIndex";
    private const string KEY_POINTS_TO_WIN_INDEX = "PointsToWinIndex";
    private const string KEY_POINTS_TO_WIN_VALUE = "PointsToWin";
    private const string KEY_BOUT_LENGTH_INDEX = "BoutLengthIndex";
    private const string KEY_BOUT_LENGTH_VALUE = "BoutLength";

    public void Awake()
    {
        dM = PlayerDataManager.GetInstance();

        // listen for dropdown changes
        opponentTypeDropdown.onValueChanged.AddListener(OnOpponentTypeChanged);
        gameModeDropdown.onValueChanged.AddListener(OnGameModeChanged);
        pointsToWinDropdown.onValueChanged.AddListener(OnPointsToWinChanged);
        boutLengthDropdown.onValueChanged.AddListener(OnBoutLengthChanged);

        // Setup initial opponent type
        string selected = opponentTypeDropdown.options[opponentTypeDropdown.value].text;
        PlayerPrefs.SetString(KEY_OPPONENT_TYPE, selected);
        PlayerPrefs.Save();

        isAI = selected == "AI";

        if (isAI)
        {
            SetupAI();
            dM.p2 = "AI";
        }
        else
        {
            SetupPlayer2();
            dM.p2 = "";
        }

        PopulateScrollView();
        OnGameModeChanged(gameModeDropdown.value);
    }

    private void OnOpponentTypeChanged(int index)
    {
        string selected = opponentTypeDropdown.options[index].text;

        // Save selection
        PlayerPrefs.SetString(KEY_OPPONENT_TYPE, selected);
        PlayerPrefs.Save();

        isAI = selected == "AI";

        if (isAI)
        {
            SetupAI();
            dM.p2 = "AI";
        }
        else
        {
            SetupPlayer2();
            dM.p2 = "";
        }

        PopulateScrollView();
    }

    public void PopulateScrollView()
    {
        ClearScrollViewContent();
        List<string> playerNames = dM.GetAllPlayerNames();
        dM.ClearSelectedPlayers();

        if (isAI)
            dM.p2 = "AI";

        startMatchButton.GetComponent<Button>().interactable = false;

        foreach (string playerName in playerNames)
        {
            MyButton buttonP1 = Instantiate(buttonPrefab, contentP1).GetComponent<MyButton>();
            buttonP1.playerId = 1;
            MyButton buttonP2 = Instantiate(buttonPrefab, contentP2).GetComponent<MyButton>();
            buttonP2.playerId = 2;

            buttonP1.SetText(playerName);
            buttonP2.SetText(playerName);

            buttonP1.callback = ButtonClicked;
            buttonP2.callback = ButtonClicked;
        }
    }

    private void Update()
    {
        string p1 = dM.p1, p2 = dM.p2;

        if (isAI)
        {
            if (buttonList[0].GetComponent<ReadyButton>().isReady &&
                !string.IsNullOrEmpty(p1))
            {
                startMatchButton.GetComponent<Button>().interactable = true;
                return;
            }
        }
        else
        {
            if (buttonList[0].GetComponent<ReadyButton>().isReady &&
                buttonList[1].GetComponent<ReadyButton>().isReady)
            {
                if (!string.IsNullOrEmpty(p1) &&
                    !string.IsNullOrEmpty(p2) &&
                    !string.Equals(p1, p2, System.StringComparison.OrdinalIgnoreCase))
                {
                    startMatchButton.GetComponent<Button>().interactable = true;
                    return;
                }
            }
        }

        startMatchButton.GetComponent<Button>().interactable = false;
    }

    private void SetupAI()
    {
        selection2.SetActive(false);
        contentP2.gameObject.SetActive(false);

        if (fencers != null && fencers.Count > 1)
            fencers[1].SetActive(true);

        p2HeaderText.text = "AI";
    }

    private void SetupPlayer2()
    {
        selection2.SetActive(true);
        contentP2.gameObject.SetActive(true);

        if (fencers != null && fencers.Count > 1)
            fencers[1].SetActive(false);

        p2HeaderText.text = "Player 2";
    }

    private void ClearScrollViewContent()
    {
        foreach (Transform child in contentP1)
            Destroy(child.gameObject);

        foreach (Transform child in contentP2)
            Destroy(child.gameObject);
    }

    public void ButtonClicked(string buttonText, int playerNum)
    {
        if (playerNum == 1)
        {
            if (!fencers[0].activeSelf)
                fencers[0].SetActive(true);

            buttonList[0].GetComponent<ReadyButton>().isReady = false;
            dM.p1 = buttonText;
            p1HeaderText.text = buttonText;
        }
        else if (playerNum == 2 && !isAI)
        {
            if (!fencers[1].activeSelf)
                fencers[1].SetActive(true);

            buttonList[1].GetComponent<ReadyButton>().isReady = false;
            dM.p2 = buttonText;
            p2HeaderText.text = buttonText;
        }
    }

    private void OnGameModeChanged(int index)
    {
        bool isFirstToX = (index == 0);

        pointsToWinText.gameObject.SetActive(isFirstToX);
        pointsToWinDropdown.gameObject.SetActive(isFirstToX);

        boutLengthText.gameObject.SetActive(!isFirstToX);
        boutLengthDropdown.gameObject.SetActive(!isFirstToX);

        // Save game mode
        PlayerPrefs.SetInt(KEY_GAME_MODE_INDEX, index);
        PlayerPrefs.Save();
    }

    private void OnPointsToWinChanged(int index)
    {
        PlayerPrefs.SetInt(KEY_POINTS_TO_WIN_INDEX, index);

        if (int.TryParse(pointsToWinDropdown.options[index].text, out int value))
            PlayerPrefs.SetInt(KEY_POINTS_TO_WIN_VALUE, value);

        PlayerPrefs.Save();
    }

    private void OnBoutLengthChanged(int index)
    {
        PlayerPrefs.SetInt(KEY_BOUT_LENGTH_INDEX, index);

        if (int.TryParse(boutLengthDropdown.options[index].text, out int value))
            PlayerPrefs.SetInt(KEY_BOUT_LENGTH_VALUE, value);

        PlayerPrefs.Save();
    }
}
