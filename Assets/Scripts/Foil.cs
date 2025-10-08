using System.Collections;
using UnityEngine;

public class Foil : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.ToLower().Equals("foil"))
        {
            GameObject enemy_player = other.gameObject.transform.root.gameObject;
            //enemy_player.GetComponent<Animator>().SetTrigger("Hit"); //will do this eventually :)
            StartCoroutine(pausePlayerMovement(enemy_player));
        }
    }

    private IEnumerator pausePlayerMovement(GameObject enemy)
    {
        enemy.GetComponent<S_A_Locomotion>().enabled = false;
        enemy.GetComponent<S_A_FencingMoveController>().enabled = false;
        yield return new WaitForSeconds(2f);
        enemy.GetComponent<S_A_Locomotion>().enabled = true;
        enemy.GetComponent<S_A_FencingMoveController>().enabled = true;
    }
}
