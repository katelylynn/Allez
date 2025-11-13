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

    public static void StartGame(string opponentType)
    {
        PlayerPrefs.SetString("OpponentType", opponentType.ToString());
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public static void QuitGame()
    {
        Application.Quit(); //for builds
        EditorApplication.isPlaying = false; // for editor
    }
}
