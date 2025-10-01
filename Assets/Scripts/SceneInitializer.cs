using UnityEngine;
using System.Collections.Generic;

public class SceneInitializer : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    public GameObject fencerPrefab;
    public GameObject environmentPrefab;

    void Awake()
    {
        GameObject g = Instantiate(gameManagerPrefab);
        g.name = "GameManager";

        GameObject f0 = Instantiate(fencerPrefab);
        f0.name = "Fencer0";
        f0.GetComponent<Fencer>().Initialize(0);

        GameObject f1 = Instantiate(fencerPrefab);
        f1.name = "Fencer1";
        f1.GetComponent<Fencer>().Initialize(1);

        GameObject env = Instantiate(environmentPrefab);
        env.name = "Environment";
    }
}
