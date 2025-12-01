using UnityEngine;

public class FencerColorChangeForMainMenu : MonoBehaviour
{
    void Start()
    {
        GetComponent<S_A_SkinnedOutfitColorChange>().ChangeOutfitColor(0, new Color(0.15f, 0.24f, 0.67f));
    }
}
