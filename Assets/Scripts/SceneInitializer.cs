using UnityEngine;
using System.Collections.Generic;

public class SceneInitializer : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    public GameObject fencerPrefab;
    public GameObject combatManagerPrefab;
    public GameObject environmentPrefab;

    public FencerType fencer0Type;
    public FencerType fencer1Type;

    void Awake()
    {
        // Instantiate GameObjects

        GameObject g = Instantiate(gameManagerPrefab);
        g.name = "GameManager";

        GameObject f0 = Instantiate(fencerPrefab);
        f0.name = "Fencer0";
        f0.GetComponent<Fencer>().Initialize(FencerId.Fencer0, fencer0Type);

        GameObject f1 = Instantiate(fencerPrefab);
        f1.name = "Fencer1";
        f1.GetComponent<Fencer>().Initialize(FencerId.Fencer1, fencer1Type);

        GameObject cm = Instantiate(combatManagerPrefab);
        cm.name = "CombatManager";
        cm.GetComponent<CombatManager>().Initialize(f0.GetComponent<Fencer>(), f1.GetComponent<Fencer>());

        GameObject env = Instantiate(environmentPrefab);
        env.name = "Environment";

        // Start the fight

        g.GetComponent<GameManager>().StartDuel();
    }
}
