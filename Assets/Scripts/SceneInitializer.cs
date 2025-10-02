using UnityEngine;
using System.Collections.Generic;

public class SceneInitializer : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    public GameObject fencerPrefab;
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
        f0.GetComponent<Fencer>().Initialize(0, fencer0Type);

        GameObject f1 = Instantiate(fencerPrefab);
        f1.name = "Fencer1";
        f1.GetComponent<Fencer>().Initialize(1, fencer1Type);

        GameObject env = Instantiate(environmentPrefab);
        env.name = "Environment";

        // Start the fight

        g.GetComponent<GameManager>().StartDuel();
    }
}
