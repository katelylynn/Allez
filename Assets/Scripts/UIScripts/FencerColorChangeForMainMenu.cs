using UnityEngine;

public class FencerColorChangeForMainMenu : MonoBehaviour
{
    void Start()
    {
        Debug.Log(gameObject.name);
        GetComponent<Fencer>().ChangeOutfitColor(0, GlobalColours.FencerBlue);
    }
}
