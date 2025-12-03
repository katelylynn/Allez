// using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public class SceneSwapper : MonoBehaviour
{
    private static Stack<string> sceneHistory = new Stack<string>();

    public static void ChangeScene(string sceneName)
    {
        Scene current = SceneManager.GetActiveScene();
        if (current.IsValid())
            sceneHistory.Push(current.name);

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public static void ToggleAdditiveScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        // If scene is loaded → unload it
        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
            return;
        }

        // Otherwise load additively & push history
        Scene current = SceneManager.GetActiveScene();
        if (current.IsValid())
            sceneHistory.Push(current.name);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public static void GoBack()
    {
        if (sceneHistory.Count == 0)
        {
            Debug.Log("No previous scene found in history.");
            return;
        }

        string previousScene = sceneHistory.Pop();
        SceneManager.LoadScene(previousScene);
    }

    public void GoBackInstance()
    {
        GoBack();
    }

    public static void QuitGame()
    {
        // Start the delay coroutine using any active MonoBehaviour
        Time.timeScale = 1.0f;
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
