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

    public void Awake()
    {
        dM = PlayerDataManager.GetInstance();

        // Load saved opponent type (default to Player)
        string savedType = PlayerPrefs.GetString("OpponentType", "Player");
        isAI = savedType == "AI";

        // Listen for dropdown changes
        opponentTypeDropdown.onValueChanged.AddListener(OnOpponentTypeChanged);

        // Listen for changes
        gameModeDropdown.onValueChanged.AddListener(OnGameModeChanged);

        // Sync opponent type dropdown to saved value
        int dropdownIndex = opponentTypeDropdown.options.FindIndex(o => o.text == savedType);
        if (dropdownIndex < 0) dropdownIndex = 0;
        opponentTypeDropdown.value = dropdownIndex;
        opponentTypeDropdown.RefreshShownValue();

        // Configure UI and player data based on initial type
        OnOpponentTypeChanged(dropdownIndex);

        // Set initial visibility
        OnGameModeChanged(gameModeDropdown.value);

        // Set up start match button once, branch at click time
        var btn = startMatchButton.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            if (isAI)
                SceneSwapper.ChangeSceneAI("MainScene");
            else
                SceneSwapper.ChangeScenePlayer("MainScene");
        });
    }

    private void OnOpponentTypeChanged(int index)
    {
        string selected = opponentTypeDropdown.options[index].text;

        // Save selection
        PlayerPrefs.SetString("OpponentType", selected);
        PlayerPrefs.Save();

        // Update local flag
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

        // Rebuild player lists
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
                if (!string.IsNullOrEmpty(p1) && !string.IsNullOrEmpty(p2))
                {
                    if (!string.Equals(p1, p2, System.StringComparison.OrdinalIgnoreCase))
                    {
                        startMatchButton.GetComponent<Button>().interactable = true;
                        return;
                    }
                }
            }
        }

        if (startMatchButton.GetComponent<Button>().interactable)
            startMatchButton.GetComponent<Button>().interactable = false;
    }

    private void SetupAI()
    {
        // Hide Player 2 selection UI, show AI fencer
        selection2.SetActive(false);
        contentP2.gameObject.SetActive(false);

        if (fencers != null && fencers.Count > 1 && fencers[1] != null)
            fencers[1].SetActive(true);

        p2HeaderText.text = "AI";
    }

    private void SetupPlayer2()
    {
        // Show Player 2 selection UI, hide fencer until selected/ready
        selection2.SetActive(true);
        contentP2.gameObject.SetActive(true);

        if (fencers != null && fencers.Count > 1 && fencers[1] != null)
            fencers[1].SetActive(false);

        p2HeaderText.text = "Player 2";
    }

    private void ClearScrollViewContent()
    {
        foreach (Transform child in contentP1)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in contentP2)
        {
            Destroy(child.gameObject);
        }
    }

    public void ButtonClicked(string buttonText, int playerNum)
    {
        if (playerNum == 0)
        {
            Debug.Log("Something went wrong, playerNum is 0");
        }

        if (playerNum == 1)
        {
            if (!fencers[playerNum - 1].activeSelf)
                fencers[playerNum - 1].SetActive(true);

            buttonList[playerNum - 1].GetComponent<ReadyButton>().isReady = false;
            dM.p1 = buttonText;
            p1HeaderText.text = buttonText;
        }
        else if (playerNum == 2 && !isAI)
        {
            if (!fencers[playerNum - 1].activeSelf)
                fencers[playerNum - 1].SetActive(true);

            buttonList[playerNum - 1].GetComponent<ReadyButton>().isReady = false;
            dM.p2 = buttonText;
            p2HeaderText.text = buttonText;
        }
    }

    private void OnGameModeChanged(int index)
    {
        // index 0 = First to X points
        // index 1 = Most points in X seconds
        bool isFirstToX = (index == 0);

        pointsToWinText.gameObject.SetActive(isFirstToX);
        pointsToWinDropdown.gameObject.SetActive(isFirstToX);

        boutLengthText.gameObject.SetActive(!isFirstToX);
        boutLengthDropdown.gameObject.SetActive(!isFirstToX);
    }
}
