using UnityEngine;

public class Fencer : MonoBehaviour
{
    private int playerNumber;

    public void Initialize(int pn)
    {
        playerNumber = pn;
        Debug.Log(pn);
    }
}
