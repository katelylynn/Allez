// using System.Collections;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class SwordAttack : MonoBehaviour
// {
//     private InputActions inputActions;
//     private InputAction attack = null;
//     private InputAction parryLeft = null;
//     private InputAction parryRight = null;

//     private void Awake()
//     {
//         inputActions = new InputActions();
//         if (gameObject.tag == "P1")
//         {
//             attack = inputActions.Player.P1Attack;
//             attack.performed += Attack;

//             parryLeft = inputActions.Player.P1ParryLeft;
//             parryLeft.performed += ParryLeft;

//             parryRight = inputActions.Player.P1ParryRight;
//             parryRight.performed += ParryRight;
//         }
//         if (gameObject.tag == "P2")
//         {
//             attack = inputActions.Player.P2Attack;
//             attack.performed += Attack;
//         }
//     }

//     private void OnEnable()
//     {
//         if (attack != null && parryLeft != null && parryRight != null)
//         {
//             attack.Enable();
//             parryLeft.Enable();
//             parryRight.Enable();
//         }
//     }

//     private void Attack(InputAction.CallbackContext obj)
//     {
//         StartCoroutine(actualAttack());
//     }

//     private void ParryLeft(InputAction.CallbackContext obj)
//     {
//         StartCoroutine(actualParry(true));
//     }

//     private void ParryRight(InputAction.CallbackContext obj)
//     {
//         StartCoroutine(actualParry(false));
//     }

//     private IEnumerator actualAttack()
//     {
//         float attackDistance = 2f;
//         float moveSpeed = 0.1f;

//         attack.Disable();
//         parryLeft.Disable();
//         parryRight.Disable();
//         Transform sword = transform.Find("Sword");
        
//         float newSwordPosition = sword.position.z + attackDistance;
//         float oldSwordPosition = sword.position.z;

//         while (sword.position.z < newSwordPosition)
//         {
//             sword.position = new Vector3(sword.position.x, sword.position.y, sword.position.z + moveSpeed);
//             yield return new WaitForSeconds(0.006f);
//         }
//         while (sword.position.z > oldSwordPosition)
//         {
//             sword.position = new Vector3(sword.position.x, sword.position.y, sword.position.z - moveSpeed);
//             yield return new WaitForSeconds(0.006f);
//         }
//         attack.Enable();
//         parryLeft.Enable();
//         parryRight.Enable();
//     }

//     /*
//      * -1 is left parry
//      * 1 is right parry
//      */
//     private IEnumerator actualParry(bool isLeftParry)
//     {
//         float parryDistance = 1f;
//         float moveSpeed = 0.05f;
//         if (isLeftParry)
//         {
//             parryLeft.Disable();
//             parryRight.Disable();
//             Transform sword = transform.Find("Sword");
//             float newSwordDistance = sword.position.x - parryDistance;
//             float oldSwordDistance = sword.position.x;
//             while (sword.position.x > newSwordDistance)
//             {
//                 sword.position = new Vector3(sword.position.x - moveSpeed, sword.position.y, sword.position.z);
//                 yield return new WaitForSeconds(0.001f);
//             }
//             while (sword.position.x < oldSwordDistance)
//             {
//                 sword.position = new Vector3(sword.position.x + moveSpeed, sword.position.y, sword.position.z);
//                 yield return new WaitForSeconds(0.001f);
//             }
//             parryLeft.Enable();
//             parryRight.Enable();
//         }
//         else
//         {
//             parryRight.Disable();
//             parryLeft.Disable();
//             Transform sword = transform.Find("Sword");
//             float newSwordDistance = sword.position.x + parryDistance;
//             float oldSwordDistance = sword.position.x;
//             while (sword.position.x < newSwordDistance)
//             {
//                 sword.position = new Vector3(sword.position.x + moveSpeed, sword.position.y, sword.position.z);
//                 yield return new WaitForSeconds(0.001f);
//             }
//             while (sword.position.x > oldSwordDistance)
//             {
//                 sword.position = new Vector3(sword.position.x - moveSpeed, sword.position.y, sword.position.z);
//                 yield return new WaitForSeconds(0.001f);
//             }
//             parryRight.Enable();
//             parryLeft.Enable();
//         }
//     }

// }
