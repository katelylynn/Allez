using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileSelector : MonoBehaviour
{
    PlayerDataManager dM;
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

        isAI = PlayerPrefs.GetString("OpponentType", null) == "AI";

        if (isAI) 
        {
            SetupAI();
            startMatchButton.GetComponent<Button>().onClick.AddListener(() => {
                SceneSwapper.ChangeSceneAI("MainScene");
            });
        }
        else
        {
            startMatchButton.GetComponent<Button>().onClick.AddListener(() => {
                SceneSwapper.ChangeScenePlayer("MainScene");
            });
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
        //Debug.Log("player names count: " + playerNames.Count);
        startMatchButton.GetComponent<Button>().interactable = false;
        foreach (string playerName in playerNames)
        {
            //Debug.Log("PName: " + playerName);
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
        selection2.gameObject.SetActive(false);
        fencers[1].SetActive(true);
        p2HeaderText.text = "AI";
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
        //Debug.Log("Clicked " + buttonText);
        if (playerNum == 0)
        {
            Debug.Log("Something went wrong, playerNum is 0");
        }
        if (playerNum == 1)
        {
            if (!fencers[playerNum-1].activeSelf)
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
}

