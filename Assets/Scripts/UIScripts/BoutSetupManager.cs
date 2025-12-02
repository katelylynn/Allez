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
    // public GameObject selection1; // <- UNUSED, removed
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

    public ProfileSelection scrollViewP1Nav;
    public ProfileSelection scrollViewP2Nav;

    // PlayerPrefs keys
    private const string KEY_OPPONENT_TYPE = "OpponentType";
    private const string KEY_GAME_MODE = "GameMode";
    private const string KEY_POINTS_TO_WIN = "PointsToWin";
    private const string KEY_BOUT_LENGTH = "BoutLength";

    public void Awake()
    {
        dM = PlayerDataManager.GetInstance();

        opponentTypeDropdown.onValueChanged.AddListener(OnOpponentTypeChanged);
        gameModeDropdown.onValueChanged.AddListener(OnGameModeChanged);
        pointsToWinDropdown.onValueChanged.AddListener(OnPointsToWinChanged);
        boutLengthDropdown.onValueChanged.AddListener(OnBoutLengthChanged);

        // --- Opponent type: load or save default ---
        string savedOpponent = PlayerPrefs.GetString(KEY_OPPONENT_TYPE, null);
        if (string.IsNullOrEmpty(savedOpponent))
        {
            // no pref yet → save current dropdown as default
            savedOpponent = opponentTypeDropdown.options[opponentTypeDropdown.value].text;
            PlayerPrefs.SetString(KEY_OPPONENT_TYPE, savedOpponent);
            PlayerPrefs.Save();
        }
        else
        {
            int idx = opponentTypeDropdown.options.FindIndex(o => o.text == savedOpponent);
            if (idx >= 0) opponentTypeDropdown.SetValueWithoutNotify(idx);
        }

        string selectedOpponent = opponentTypeDropdown.options[opponentTypeDropdown.value].text;
        isAI = selectedOpponent == "AI";

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

        // --- Game mode: load or save default ---
        string savedMode = PlayerPrefs.GetString(KEY_GAME_MODE, null);
        if (string.IsNullOrEmpty(savedMode))
        {
            savedMode = gameModeDropdown.options[gameModeDropdown.value].text;
            PlayerPrefs.SetString(KEY_GAME_MODE, savedMode);
            PlayerPrefs.Save();
        }
        else
        {
            int idx = gameModeDropdown.options.FindIndex(o => o.text == savedMode);
            if (idx >= 0) gameModeDropdown.SetValueWithoutNotify(idx);
        }

        // --- Points to win: load or save default ---
        int savedPoints;
        if (PlayerPrefs.HasKey(KEY_POINTS_TO_WIN))
        {
            savedPoints = PlayerPrefs.GetInt(KEY_POINTS_TO_WIN);
        }
        else
        {
            string defaultPointsStr = pointsToWinDropdown.options[pointsToWinDropdown.value].text;
            savedPoints = ExtractNumber(defaultPointsStr);
            PlayerPrefs.SetInt(KEY_POINTS_TO_WIN, savedPoints);
            PlayerPrefs.Save();
        }
        int idxPoints = pointsToWinDropdown.options.FindIndex(o => ExtractNumber(o.text) == savedPoints);
        if (idxPoints >= 0) pointsToWinDropdown.SetValueWithoutNotify(idxPoints);

        // --- Bout length: load or save default ---
        int savedLength;
        if (PlayerPrefs.HasKey(KEY_BOUT_LENGTH))
        {
            savedLength = PlayerPrefs.GetInt(KEY_BOUT_LENGTH);
        }
        else
        {
            string defaultLengthStr = boutLengthDropdown.options[boutLengthDropdown.value].text;
            savedLength = ExtractNumber(defaultLengthStr);
            PlayerPrefs.SetInt(KEY_BOUT_LENGTH, savedLength);
            PlayerPrefs.Save();
        }
        int idxLength = boutLengthDropdown.options.FindIndex(o => ExtractNumber(o.text) == savedLength);
        if (idxLength >= 0) boutLengthDropdown.SetValueWithoutNotify(idxLength);

        PopulateScrollView();
        OnGameModeChanged(gameModeDropdown.value);
    }

    private int ExtractNumber(string text)
    {
        string digits = "";
        foreach (char c in text)
        {
            if (char.IsDigit(c))
                digits += c;
        }

        if (int.TryParse(digits, out int result))
            return result;

        return 0;
    }

    private void OnOpponentTypeChanged(int index)
    {
        string selected = opponentTypeDropdown.options[index].text;

        PlayerPrefs.SetString(KEY_OPPONENT_TYPE, selected);
        PlayerPrefs.Save();

        isAI = selected == "AI";

        if (isAI)
        {
            SetupAI();
            dM.p2 = "AI";
            buttonList[1].GetComponent<ReadyButton>().isReady = false;
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
        string tempP1 = null;

        if (dM.p1 != null) //shitty hacky fix to prevent ready up bug when opponent type changes. part 1
        {
            tempP1 = dM.p1;
        }

        dM.ClearSelectedPlayers();

        if (tempP1 != null) //part 2 of shitty fix, should redo later. does reset clear selected player even need to be called above here?
        {
            dM.p1 = tempP1;
        }

        if (isAI)
            dM.p2 = "AI";

        startMatchButton.GetComponent<Button>().interactable = false;

        // Track the first buttons we create so we can give them to the scroll view nav scripts
        Selectable firstP1 = null;
        Selectable firstP2 = null;

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

            // Grab the Selectable on these (usually a Button component)
            Selectable s1 = buttonP1.GetComponent<Selectable>();
            Selectable s2 = buttonP2.GetComponent<Selectable>();

            if (firstP1 == null && s1 != null)
                firstP1 = s1;

            if (firstP2 == null && s2 != null)
                firstP2 = s2;
        }

        // Tell the ScrollViewEnterNavigation scripts what their first child item is
        if (scrollViewP1Nav != null)
            scrollViewP1Nav.SetFirstItem(firstP1);

        if (scrollViewP2Nav != null)
            scrollViewP2Nav.SetFirstItem(firstP2);
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

        PlayerPrefs.SetString(KEY_GAME_MODE, gameModeDropdown.options[index].text);
        PlayerPrefs.Save();

        UpdateNavigation();
    }

    private void OnPointsToWinChanged(int index)
    {
        int value = ExtractNumber(pointsToWinDropdown.options[index].text);
        PlayerPrefs.SetInt(KEY_POINTS_TO_WIN, value);
        PlayerPrefs.Save();
    }

    private void OnBoutLengthChanged(int index)
    {
        int value = ExtractNumber(boutLengthDropdown.options[index].text);
        PlayerPrefs.SetInt(KEY_BOUT_LENGTH, value);
        PlayerPrefs.Save();
    }

    private void UpdateNavigation()
    {
        // Decide which field is the "middle" one based on game mode
        bool isFirstToX = (gameModeDropdown.value == 0);
        Selectable midSelectable = isFirstToX
            ? (Selectable)pointsToWinDropdown
            : (Selectable)boutLengthDropdown;

        // --- GameModeDropdown: Down goes to midSelectable ---
        Navigation gameModeNav = gameModeDropdown.navigation;
        gameModeNav.mode = Navigation.Mode.Explicit;
        gameModeNav.selectOnDown = midSelectable;
        gameModeDropdown.navigation = gameModeNav;

        // --- OpponentTypeDropdown: Up comes from midSelectable ---
        Navigation opponentNav = opponentTypeDropdown.navigation;
        opponentNav.mode = Navigation.Mode.Explicit;
        opponentNav.selectOnUp = midSelectable;
        opponentTypeDropdown.navigation = opponentNav;
    }
}
