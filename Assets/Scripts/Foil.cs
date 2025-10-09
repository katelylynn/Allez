using System.Collections;
using UnityEngine;

public class Foil : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject enemy_player = other.gameObject.transform.root.gameObject; // gets the top level parent gameObject of the foil (the player)
        Animator enemy_animator = enemy_player.GetComponent<Animator>();

        GameObject self_player = gameObject.transform.root.gameObject;
        if (other.gameObject.name.ToLower().Equals("foil"))
        {
            if (enemy_animator.GetCurrentAnimatorStateInfo(0).IsName("Parry Left"))
            {
                //enemy_player.GetComponent<Animator>().SetTrigger("Parried"); //will do this eventually :)
                Debug.Log("Player " + self_player.name + " is parried!");
                StartCoroutine(pausePlayerMovement(self_player)); //remove this when animation is added
            }
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
