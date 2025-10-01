using UnityEngine;
using System.Collections.Generic;

public class SceneInitializer : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    public GameObject fencerPrefab;

    void Awake()
    {
        GameObject g = Instantiate(gameManagerPrefab);
        g.name = "GameManager";

        GameObject f1 = Instantiate(fencerPrefab);
        f1.name = "Fencer1";
        f1.GetComponent<Fencer>().Initialize(1);

        GameObject f2 = Instantiate(fencerPrefab);
        f2.name = "Fencer2";
        f2.GetComponent<Fencer>().Initialize(2);
    }
}
