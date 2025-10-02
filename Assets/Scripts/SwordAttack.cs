using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAttack : MonoBehaviour
{
    private InputActions inputActions;
    private InputAction attack = null;
    private InputAction parryLeft = null;
    private InputAction parryRight = null;

    private void Awake()
    {
        inputActions = new InputActions();
        if (gameObject.tag == "P1")
        {
            attack = inputActions.Player.P1Attack;
            attack.performed += Attack;

            parryLeft = inputActions.Player.P1ParryLeft;
            parryLeft.performed += ParryLeft;

            parryRight = inputActions.Player.P1ParryRight;
            parryRight.performed += ParryRight;
        }
        if (gameObject.tag == "P2")
        {
            attack = inputActions.Player.P2Attack;
            attack.performed += Attack;
        }
    }

    private void OnEnable()
    {
        if (attack != null && parryLeft != null && parryRight != null)
        {
            attack.Enable();
            parryLeft.Enable();
            parryRight.Enable();
        }
    }

    private void Attack(InputAction.CallbackContext obj)
    {
        StartCoroutine(actualAttack());
    }

    private void ParryLeft(InputAction.CallbackContext obj)
    {
        StartCoroutine(actualParry(true));
    }

    private void ParryRight(InputAction.CallbackContext obj)
    {
        StartCoroutine(actualParry(false));
    }

    private IEnumerator actualAttack()
    {
        attack.Disable();
        parryLeft.Disable();
        parryRight.Disable();
        Transform sword = transform.Find("Sword");
        sword.position += sword.forward * 2f;
        yield return new WaitForSeconds(0.5f);
        sword.position -= sword.forward * 2f;
        Debug.Log(sword.gameObject.name);
        attack.Enable();
        parryLeft.Enable();
        parryRight.Enable();
    }

    private IEnumerator actualParry(bool isLeftParry)
    {
        float parryDistance = 1f;
        if (isLeftParry)
        {
            parryLeft.Disable();
            parryRight.Disable();
            Transform sword = transform.Find("Sword");
            sword.position = new Vector3(sword.position.x - parryDistance, sword.position.y, sword.position.z);
            yield return new WaitForSeconds(0.3f);
            sword.position = new Vector3(sword.position.x + parryDistance, sword.position.y, sword.position.z);
            parryLeft.Enable();
            parryRight.Enable();
        }
        else
        {
            parryRight.Disable();
            parryLeft.Disable();
            Transform sword = transform.Find("Sword");
            sword.position = new Vector3(sword.position.x + parryDistance, sword.position.y, sword.position.z);
            yield return new WaitForSeconds(0.3f);
            sword.position = new Vector3(sword.position.x - parryDistance, sword.position.y, sword.position.z);
            parryRight.Enable();
            parryLeft.Enable();
        }
    }
}
