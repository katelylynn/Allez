using UnityEngine;
using System.Collections.Generic;

public class SceneInitializer : MonoBehaviour
{
    public List<GameObject> prefabsToSpawn;

    void Awake()
    {
        foreach (GameObject prefab in prefabsToSpawn)
            Instantiate(prefab);
    }
}
