using TMPro;
using UnityEngine;

public class ScoreCanvasColorSettings : MonoBehaviour
{
    [SerializeField]
    TMP_Text p1Name;
    [SerializeField]
    TMP_Text p2Name;

    private void Start()
    {
        p1Name.color = GlobalColours.Blue;
        p2Name.color = GlobalColours.Red;
    }
}
