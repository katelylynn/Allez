using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public static void ChangeScenePlayer(string sceneName)
    {
        PlayerPrefs.SetString("OpponentType", "Player");
        PlayerPrefs.Save();

        ChangeScene(sceneName);
    }

    public static void ChangeSceneAI(string sceneName)
    {
        PlayerPrefs.SetString("OpponentType", "AI");
        PlayerPrefs.Save();

        ChangeScene(sceneName);
    }

    public static void QuitGame()
    {
        Application.Quit(); //for builds
        EditorApplication.isPlaying = false; // for editor
    }
}
