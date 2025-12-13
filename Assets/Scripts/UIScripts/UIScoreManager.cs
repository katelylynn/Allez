/*
    UI Score Manager
    Manages the display of the score.
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScoreManager : MonoBehaviour
{
    public GameObject winPip;
    public GameObject pip;
    public GameManager gm;
    public TMP_Text playerName;
    private Transform scoreUI;
    public bool isP1 = true;
    private int[] oldScore = {0,0};
    PlayerDataManager dm;
    public TMP_Text countdownText;

    public void Initialize(GameObject gameManager)
    {
        gm = gameManager.GetComponent<GameManager>();
        dm = PlayerDataManager.GetInstance();
        scoreUI = transform.Find("PipContainer");
    }

    public void UpdateCountdown(float timeLeft)
    {
        countdownText.text = (int) timeLeft + "s";
    }

    public void UpdateUI()
    {
        if (scoreUI == null)        
            return;

        RemoveAllChildrenUI();

        oldScore[0] = PlayerPrefs.GetInt("P1Score");
        oldScore[1] = PlayerPrefs.GetInt("P2Score");
        playerName.text = isP1 ? dm.p1 : dm.p2;
        int playerScore = isP1 ? PlayerPrefs.GetInt("P1Score") : PlayerPrefs.GetInt("P2Score");
        
        for (int i = 0; i < gm.pointsToWin; i++)
        {
            // have to do this because you cant use prefabs directly as child objects
            GameObject prefab;
            GameObject newPip;
            if (playerScore > i){
                prefab = winPip;
                newPip = Instantiate(prefab);       
            }
            else
            {
                prefab = pip;
                newPip = Instantiate(prefab);
            }                           
            newPip.transform.SetParent(scoreUI, false);
        }
    }

    private void RemoveAllChildrenUI()
    {
        if (scoreUI == null)
            return;

        for (int i = scoreUI.childCount - 1; i >= 0; i--)
        {
            Destroy(scoreUI.GetChild(i).gameObject);
        }
    }
}
