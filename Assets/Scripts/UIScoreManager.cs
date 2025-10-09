using UnityEngine;
using UnityEngine.UIElements;

public class UIScoreManager : MonoBehaviour
{
    public GameObject winPip;
    public GameObject pip;
    public GameManager gm;
    public Transform scoreUI;
    public bool isP1 = true;
    
    void OnEnable()
    {
        for(int i = 0; i < gm.pointsToWin; i++)
        {
            //GameObject newPip = Instantiate((gm.score[0] > i ? winPip : pip), scoreUI);
            GameObject newPip = Instantiate((gm.score[System.Convert.ToInt32(isP1)] > i ? winPip : pip), scoreUI);
        }
    }
}
