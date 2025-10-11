using UnityEngine;

public class AllezSceneManager : MonoBehaviour
{
    //singleton object to transfer data between scenes
    public static AllezSceneManager Instance;
    public int[] scores = { 0,0 };

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
