using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputActions inputActions;
    private InputAction movement;
    private InputAction attack = null;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputActions();
        if(gameObject.tag == "P1")
        {
            movement = inputActions.Player.P1Movement;
            attack = inputActions.Player.P1Attack;
            attack.performed += Attack;
        }
        if(gameObject.tag == "P2")
        {
            movement = inputActions.Player.P2Movement;
        }
    }

    private void OnEnable()
    {
        movement.Enable();
        if (attack != null)
            attack.Enable();
    }

    private void FixedUpdate()
    {
        Vector2 v2 = movement.ReadValue<Vector2>();
        Vector3 v3 = new Vector3(v2.x, 0 , v2.y);

        rb.AddForce(v3, ForceMode.VelocityChange);
    }
    private void Attack(InputAction.CallbackContext obj)
    {
        //Transform sword = transform.Find("Sword");
        //sword.position += sword.forward * 5f;
        StartCoroutine(actualAttack());

        //yield return new WaitForSeconds(2f);
        //sword.position -= sword.forward * 5f;
        //Debug.Log(sword.gameObject.name);
    }

    private IEnumerator wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private IEnumerator actualAttack()
    {
        Transform sword = transform.Find("Sword");
        //sword.Translate(Vector3.forward * 5f);
        sword.position += sword.forward * 2f;

        //wait(2f);
        yield return new WaitForSeconds(0.5f);
        //sword.Translate(Vector3.back * 5f);
        sword.position -= sword.forward * 2f;
        Debug.Log(sword.gameObject.name);
        //yield return new WaitForSeconds(seconds);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Plane")
            Debug.Log(collision.gameObject.name);
    }
}
