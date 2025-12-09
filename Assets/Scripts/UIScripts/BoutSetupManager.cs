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

    public GameObject p2NewProfileInput;   // the input field container / gameObject
    public GameObject p2NewProfileButton;  // the "Create new profile" button

    // PlayerPrefs keys
    private const string KEY_OPPONENT_TYPE = "OpponentType";
    private const string KEY_GAME_MODE = "GameMode";
    private const string KEY_POINTS_TO_WIN = "PointsToWin";
    private const string KEY_BOUT_LENGTH = "BoutLength";
    private const string KEY_AI_DIFFICULTY = "AIDifficulty"; // 0=Easy,1=Normal,2=Hard

    private static readonly string[] AI_DIFFICULTIES = { "Easy", "Normal", "Hard" };

    public void Awake()
    {
        dM = PlayerDataManager.GetInstance();

        opponentTypeDropdown.onValueChanged.AddListener(OnOpponentTypeChanged);
        gameModeDropdown.onValueChanged.AddListener(OnGameModeChanged);
        pointsToWinDropdown.onValueChanged.AddListener(OnPointsToWinChanged);
        boutLengthDropdown.onValueChanged.AddListener(OnBoutLengthChanged);

        // --- AI difficulty: load index (0/1/2) and sync into data manager ---
        int savedAIDiffIndex = PlayerPrefs.GetInt(KEY_AI_DIFFICULTY, dM.aiDifficultyIndex);
        dM.aiDifficultyIndex = Mathf.Clamp(savedAIDiffIndex, 0, AI_DIFFICULTIES.Length - 1);

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
            dM.p2 = GetAIPlayer2Name();  // "Easy AI" / "Normal AI" / "Hard AI"
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

        // Set up P2 scroll view nav based on initial opponent type
        UpdateP2ScrollViewNavigation();
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

    private string GetDifficultyLabel(int index)
    {
        if (index < 0 || index >= AI_DIFFICULTIES.Length)
            index = 1; // default Normal
        return AI_DIFFICULTIES[index];
    }

    private string GetAIPlayer2Name()
    {
        // e.g. "Easy AI", "Normal AI", "Hard AI"
        return $"{GetDifficultyLabel(dM.aiDifficultyIndex)} AI";
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
            dM.p2 = GetAIPlayer2Name();   // ensure p2 gets "X AI"
            if (buttonList.Count > 1 && buttonList[1] != null)
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
            dM.p2 = GetAIPlayer2Name();   // keep P2 as "<Difficulty> AI"

        startMatchButton.GetComponent<Button>().interactable = false;

        // Track the first buttons we create so we can give them to the scroll view nav scripts
        Selectable firstP1 = null;
        Selectable firstP2 = null;

        foreach (string playerName in playerNames)
        {
            MyButton buttonP1 = Instantiate(buttonPrefab, contentP1).GetComponent<MyButton>();
            buttonP1.playerId = 1;

            buttonP1.SetText(playerName);
            buttonP1.callback = ButtonClicked;

            Selectable s1 = buttonP1.GetComponent<Selectable>();
            if (firstP1 == null && s1 != null)
                firstP1 = s1;
        }

        // --- P2: player profiles or AI difficulties ---
        if (isAI)
        {
            foreach (string diff in AI_DIFFICULTIES)
            {
                MyButton buttonP2 = Instantiate(buttonPrefab, contentP2).GetComponent<MyButton>();
                buttonP2.playerId = 2;
                buttonP2.SetText(diff);
                buttonP2.callback = ButtonClicked;

                Selectable s2 = buttonP2.GetComponent<Selectable>();
                if (firstP2 == null && s2 != null)
                    firstP2 = s2;
            }
        }
        else
        {
            foreach (string playerName in playerNames)
            {
                MyButton buttonP2 = Instantiate(buttonPrefab, contentP2).GetComponent<MyButton>();
                buttonP2.playerId = 2;

                buttonP2.SetText(playerName);
                buttonP2.callback = ButtonClicked;

                Selectable s2 = buttonP2.GetComponent<Selectable>();
                if (firstP2 == null && s2 != null)
                    firstP2 = s2;
            }
        }

        // Tell the ProfileSelection scripts what their first child item is
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
        // Show the P2 selection UI so we can choose difficulty
        if (selection2 != null)
            selection2.SetActive(true);
        if (contentP2 != null)
            contentP2.gameObject.SetActive(true);

        if (fencers != null && fencers.Count > 1)
            fencers[1].SetActive(true);

        string difficultyLabel = GetDifficultyLabel(dM.aiDifficultyIndex);
        p2HeaderText.text = $"{difficultyLabel} AI";

        // Hide new-profile controls when using AI
        if (p2NewProfileInput != null)
            p2NewProfileInput.SetActive(false);
        if (p2NewProfileButton != null)
            p2NewProfileButton.SetActive(false);

        // Hide P2 Ready button when AI
        if (buttonList != null && buttonList.Count > 1 && buttonList[1] != null)
            buttonList[1].SetActive(false);
    }

    private void SetupPlayer2()
    {
        if (selection2 != null)
            selection2.SetActive(true);
        if (contentP2 != null)
            contentP2.gameObject.SetActive(true);

        if (fencers != null && fencers.Count > 1)
            fencers[1].SetActive(false);

        p2HeaderText.text = "Player 2";

        // Show new-profile controls when using human P2
        if (p2NewProfileInput != null)
            p2NewProfileInput.SetActive(true);
        if (p2NewProfileButton != null)
            p2NewProfileButton.SetActive(true);

        // Show P2 Ready button again for human P2
        if (buttonList != null && buttonList.Count > 1 && buttonList[1] != null)
            buttonList[1].SetActive(true);
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
        else if (playerNum == 2)
        {
            if (isAI)
            {
                // Map difficulty text → index 0/1/2
                int diffIndex = System.Array.IndexOf(AI_DIFFICULTIES, buttonText);
                if (diffIndex < 0) diffIndex = 1; // default Normal

                dM.aiDifficultyIndex = diffIndex;

                string fullName = GetAIPlayer2Name(); // "<Difficulty> AI"
                dM.p2 = fullName;
                p2HeaderText.text = fullName;

                PlayerPrefs.SetInt(KEY_AI_DIFFICULTY, diffIndex);
                PlayerPrefs.Save();
            }
            else
            {
                if (!fencers[1].activeSelf)
                    fencers[1].SetActive(true);

                buttonList[1].GetComponent<ReadyButton>().isReady = false;
                dM.p2 = buttonText;
                p2HeaderText.text = buttonText;
            }
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

    /// <summary>
    /// Only P2 scroll view: when AI, Down → Start Match. When human, no special Down target.
    /// </summary>
    private void UpdateP2ScrollViewNavigation()
    {
        if (scrollViewP2Nav == null)
            return;

        Selectable sv = scrollViewP2Nav.GetComponent<Selectable>();
        if (sv == null)
            return;

        Navigation nav = sv.navigation;
        nav.mode = Navigation.Mode.Explicit;

        if (isAI && startMatchButton != null)
        {
            Selectable startSel = startMatchButton.GetComponent<Selectable>();
            nav.selectOnDown = startSel;
        }
        else
        {
            // For human P2, don't jump to Start Match from the scroll view
            nav.selectOnDown = null;
        }

        sv.navigation = nav;
    }
}
