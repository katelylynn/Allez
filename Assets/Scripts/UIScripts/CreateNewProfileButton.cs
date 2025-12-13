using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateNewProfileButton : MonoBehaviour
{
    public TMP_InputField input;
    public BoutSetupManager boutSetupManager;
    PlayerDataManager dM;
    public Button b;
    public Transform content;

    public void Awake()
    {
        dM = PlayerDataManager.dM;
        b.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if(dM == null)
        {
            dM = PlayerDataManager.dM;
        }
        if (!string.IsNullOrEmpty(input.text))
        {
            dM.AddNewPlayer(input.text);
            boutSetupManager.PopulateScrollView();
        }
        else
        {
            Debug.Log("input is empty");
        }
        
    }
}
