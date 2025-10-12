using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private Fencer fencer0;
    private Fencer fencer1;

    public void Start()
    {
        EventManager.ParrySuccess += HandleParrySuccess;
    }

    public void Initialize(Fencer f0, Fencer f1)
    {
        fencer0 = f0;
        fencer1 = f1;
    }

    private void HandleParrySuccess()
    {
        if (fencer0.GetStateSnapshot(1).IsName("ParryLeft"))
            Debug.Log("Fencer 0 parries Fencer 1!");
        else if (fencer1.GetStateSnapshot(1).IsName("ParryLeft"))
            Debug.Log("Fencer 1 parries Fencer 0!");
    }
}
