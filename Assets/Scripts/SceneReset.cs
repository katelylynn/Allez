using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReset : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("P1") || collision.gameObject.CompareTag("P2"))
        {
            SceneManager.LoadScene("TScene");
        }
    }
}
