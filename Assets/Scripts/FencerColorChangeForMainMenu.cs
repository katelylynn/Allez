using UnityEngine;

public class FencerColorChangeForMainMenu : MonoBehaviour
{
    void Start()
    {
        GetComponent<S_A_SkinnedOutfitColorChange>().ChangeOutfitColor(0, GlobalColours.FencerBlue);
    }
}
