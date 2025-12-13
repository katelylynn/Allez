using UnityEngine;

public class FencerColorChangeForMainMenu : MonoBehaviour
{
    void Start()
    {
        GetComponent<Fencer>().ChangeOutfitColor(0, GlobalColours.FencerBlue);
    }
}
