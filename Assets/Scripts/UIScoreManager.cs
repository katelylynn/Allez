using UnityEngine;

public class UIScoreManager : MonoBehaviour
{
    public GameObject winPip;
    public GameObject pip;
    public GameManager gm;
    private Transform scoreUI;
    public bool isP1 = true;

    public void Initialize(GameObject gameManager)
    {
        gm = gameManager.GetComponent<GameManager>();
        scoreUI = transform.Find("PipContainer");
    }

    public void UpdateUI()
    {
        if (scoreUI == null)        
            return;
        

        RemoveAllChildrenUI();

        int playerScore = isP1 ? PlayerPrefs.GetInt("P1Score") : PlayerPrefs.GetInt("P2Score");
        for (int i = 0; i < gm.pointsToWin; i++)
        {
            //have to do this because you cant use prefabs directly as child objects
            GameObject prefab = (playerScore > i) ? winPip : pip;
            GameObject newPip = Instantiate(prefab);
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
