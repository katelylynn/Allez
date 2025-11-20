// using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class SceneSwapper : MonoBehaviour
{
    public static void ChangeScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public static void AdditiveChangeScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public static void QuitGame()
    {
        // Start the delay coroutine using any active MonoBehaviour
        var instance = new GameObject("QuitHelper").AddComponent<SceneSwapper>();
        instance.StartCoroutine(instance.QuitAfterDelay());
    }

    private IEnumerator QuitAfterDelay()
    {
        Debug.Log("Quitting in 1 second...");
        yield return new WaitForSeconds(1f);

        #if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Stop play mode in editor
        #else
        Application.Quit(); // Quit build
        #endif

        Debug.Log("Game closed.");
    }
}
