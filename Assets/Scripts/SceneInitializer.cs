using UnityEngine;
using System.Collections.Generic;

public class SceneInitializer : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    public GameObject fencerPrefab;

    void Awake()
    {
        Instantiate(gameManagerPrefab);

        GameObject f1 = Instantiate(fencerPrefab);
        f1.GetComponent<Fencer>().Initialize(1);

        GameObject f2 = Instantiate(fencerPrefab);
        f2.GetComponent<Fencer>().Initialize(2);
    }
}
