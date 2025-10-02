//Old code using original inputactions script and assinging controls based on tags of play objects. Leaving it just incase we need to switch back for some reason

//using System.Collections;
//using System.Runtime.CompilerServices;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.Interactions;

//public class PlayerController : MonoBehaviour
//{
//    private InputActions inputActions;
//    private InputAction movement;
//    private InputAction multitap;
//    private InputAction tilt;
//    private Rigidbody rb;
//    //private bool isDashing = false; //for coroutine dashing

//    [Header("Dash Settings")]
//    //public float dashDuration = 0.2f; //setting for duration of coroutine dashes
//    public float lungeStrength = 50f;
//    public float backstepStrength = 20f;

//    [Header("Player Movement Settings")]
//    public float acceleration = 2f;
//    public float deceleration = 10f;
//    public float maxSpeed = 10f;

//    [Header("Player Tilt Settings")]
//    public float tiltSpeed = 360f;
//    public float maxTiltAngle = 30f;
//    private Quaternion rot;
//    private void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        inputActions = new InputActions();
//        rot = transform.rotation;
//        if (gameObject.tag == "P1")
//        {
//            movement = inputActions.Player.P1Movement;
//            multitap = inputActions.Player.P1Lunge;
//            tilt = inputActions.Player.P1Tilt;
//        }
//        if (gameObject.tag == "P2")
//        {
//            movement = inputActions.Player.P2Movement;
//            multitap = inputActions.Player.P2Lunge;
//            tilt = inputActions.Player.P2Tilt;
//        }
//    }

//    private void OnEnable()
//    {
//        movement.Enable();
//        //tilt.performed += OnTiltHold;
//        tilt.Enable();
//        multitap.started += OnMultiTapStarted;
//        multitap.performed += OnMultiTapPerformed;
//        multitap.canceled += OnMultiTapCanceled;
//        multitap.Enable();

//    }

//    private void OnDisable()
//    {
//        movement.Disable();
//        multitap.Disable();
//        tilt.Disable();
//    }

//    private void FixedUpdate()
//    {
//        movePlayer();
//        tiltModel();
//    }
//    private void movePlayer()
//    {
//        //if (isDashing) return;
//        float movementInput = movement.ReadValue<float>();
//        Vector3 v3 = new(0, 0, movementInput);
//        rb.AddRelativeForce(v3 * acceleration, ForceMode.VelocityChange);
//        if (v3 != Vector3.zero)
//        {
//            if (rb.linearVelocity.magnitude > maxSpeed)
//            {
//                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
//            }
//        }
//        else
//        {
//            //rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * deceleration); //Alternative way to control player slow down, using linear interpolation
//            rb.AddForce(-rb.linearVelocity * deceleration, ForceMode.Acceleration);
//        }
//    }
//    private void tiltModel()
//    {
//        float tiltInput = tilt.ReadValue<float>();
//        float targetAngle = tiltInput * maxTiltAngle;
//        Quaternion targetRotation = rot * Quaternion.Euler(0f, 0f, -targetAngle);
//        float t = tiltSpeed * Time.fixedDeltaTime;
//        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
//    }

//    //private void OnTiltHold(InputAction.CallbackContext context)
//    //{
//    //    if(context.control == Keyboard.current.aKey || context.control == Keyboard.current.leftArrowKey)
//    //    {
//    //        Quaternion deltaRotation = Quaternion.Euler(tiltVec * Time.fixedDeltaTime);
//    //        rb.MoveRotation(rb.rotation * deltaRotation);
//    //    }
//    //    else if(context.control == Keyboard.current.aKey || context.control == Keyboard.current.leftArrowKey)
//    //    {
//    //        Quaternion deltaRotation = Quaternion.Euler(-tiltVec * Time.fixedDeltaTime);
//    //        rb.MoveRotation(rb.rotation * deltaRotation);
//    //    }
//    //}
//    private void OnMultiTapStarted(InputAction.CallbackContext context)
//    {
//        //not needed atm
//    }

//    private void OnMultiTapPerformed(InputAction.CallbackContext context)
//    {
//        if (context.control == Keyboard.current.wKey || context.control == Keyboard.current.upArrowKey || context.control == Gamepad.current.buttonNorth)
//        {
//            Debug.Log("LUNGE");
//            Lunge(transform.forward);
//            //StartCoroutine(DashCoroutine(transform.forward, lungeSpeed));
//        }
//        else if(context.control == Keyboard.current.sKey || context.control == Keyboard.current.downArrowKey || context.control == Gamepad.current.buttonEast)
//        {
//            Debug.Log("BACKSTEP");
//            Backstep(transform.forward);
//            //StartCoroutine(DashCoroutine(-transform.forward, backstepSpeed));
//        }
//    }

//    private void OnMultiTapCanceled(InputAction.CallbackContext context)
//    {
//        //not needed atm
//    }

//    private void Lunge(Vector3 direction)
//    {
//        rb.linearVelocity = Vector3.zero;
//        rb.AddForce(direction.normalized * lungeStrength, ForceMode.VelocityChange);
//    }
//    private void Backstep(Vector3 direction)
//    {
//        rb.linearVelocity = Vector3.zero;    
//        rb.AddForce(direction.normalized * -backstepStrength, ForceMode.VelocityChange);
//    }

//    //Alternative way to do dashing, using coroutine
//    //private IEnumerator DashCoroutine(Vector3 direction, float lungeStrength)
//    //{
//    //    isDashing = true;

//    //    float elapsed = 0f;
//    //    while (elapsed < dashDuration)
//    //    {
//    //        rb.linearVelocity = direction.normalized * lungeStrength;
//    //        elapsed += Time.deltaTime;
//    //        yield return null;
//    //    }

//    //    isDashing = false;
//    //}
//}

//New playercontroller script that works with player input manager.
// P1 controls (needs to be a gamepad)
// LS up = forward, Y/triangle = lunge
// LS down = back, B/circle = backstep
// LS left = tilt left
// LS right = tilt right
// P2 controls
// W = forward, double tap to lunge
// S = back, double tap to backstep
// A = tilt left
// D = tilt right

//=======
//using System.Collections;
//>>>>>>> development
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
//<<<<<<< taylor_movement
    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction tiltAction;
    private InputAction lungeAction;
    private InputAction backstepAction;

    [Header("Dash Settings")]
    public float lungeStrength = 50f;
    public float backstepStrength = 20f;

    [Header("Player Movement Settings")]
    public float acceleration = 2f;
    public float deceleration = 10f;
    public float maxSpeed = 10f;

    [Header("Player Tilt Settings")]
    public float tiltSpeed = 360f;
    public float maxTiltAngle = 30f;
    public float tiltDeadzone = 0.2f;
    private Quaternion baseRotation;
    private float moveInput;
    private float tiltInput;
// =======
//     private InputActions inputActions;
//     private InputAction movement;
//     private InputAction attack = null;
//     Rigidbody rb;
// >>>>>>> development

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        baseRotation = transform.rotation;
    }

    private void OnEnable()
    {
        InputActionMap map = playerInput.actions.FindActionMap("Player", true);

        moveAction = map.FindAction("Move", true);
        tiltAction = map.FindAction("Tilt", true);
        lungeAction = map.FindAction("Lunge", true);
        backstepAction = map.FindAction("Backstep", true);

        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;
        tiltAction.performed += OnTiltPerformed;
        tiltAction.canceled += OnTiltCanceled;
        lungeAction.performed += OnLungePerformed;
        backstepAction.performed += OnBackstepPerformed;

        map.Enable();

        Debug.Log($"{name} using {playerInput.currentControlScheme} devices: {string.Join(", ", playerInput.devices)}");
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;
        tiltAction.performed -= OnTiltPerformed;
        tiltAction.canceled -= OnTiltCanceled;
        lungeAction.performed -= OnLungePerformed;
        backstepAction.performed -= OnBackstepPerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<float>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => moveInput = 0f;

    private void OnTiltPerformed(InputAction.CallbackContext ctx)
    {
        float f = ctx.ReadValue<float>();
        if (Mathf.Abs(f) < tiltDeadzone)
        {
//<<<<<<< taylor_movement
            tiltInput = 0f;
// =======
//             movement = inputActions.Player.P1Movement;
//             attack = inputActions.Player.P1Attack;
//             attack.performed += Attack;
// >>>>>>> development
        }
        else
        {
            tiltInput = f > 0 ? 1f : -1f;
        }
        //if (f < 0) tiltInput *= -1;
    }

    private void OnTiltCanceled(InputAction.CallbackContext ctx) => tiltInput = 0f;

    private void OnLungePerformed(InputAction.CallbackContext ctx) => lunge(transform.forward);
    private void OnBackstepPerformed(InputAction.CallbackContext ctx) => backstep(transform.forward);

    private void FixedUpdate()
    {
//<<<<<<< taylor_movement
        movePlayer();
        tiltModel();
    }

    private void movePlayer()
    {
        Vector3 localZ = new Vector3(0f, 0f, moveInput);
        rb.AddRelativeForce(localZ * acceleration, ForceMode.VelocityChange);

        if (Mathf.Abs(moveInput) > 0f)
        {
            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        else
        {
            rb.AddForce(-rb.linearVelocity * deceleration, ForceMode.Acceleration);
        }
// =======
//         movement.Enable();
//         if (attack != null)
//             attack.Enable();
// >>>>>>> development
    }

    private void tiltModel()
    {
        float targetAngle = tiltInput * maxTiltAngle;
        Quaternion target = baseRotation * Quaternion.Euler(0f, 0f, -targetAngle);
        float t = tiltSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, target, t);
    }

    private void lunge(Vector3 direction)
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction.normalized * lungeStrength, ForceMode.VelocityChange);
    }

    private void backstep(Vector3 direction)
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction.normalized * -backstepStrength, ForceMode.VelocityChange);
    }
//<<<<<<< taylor_movement

// =======
//     private void Attack(InputAction.CallbackContext obj)
//     {
//         StartCoroutine(actualAttack());
//     }

//     private IEnumerator actualAttack()
//     {
//         Transform sword = transform.Find("Sword");
//         sword.position += sword.forward * 2f;
//         yield return new WaitForSeconds(0.5f);
//         sword.position -= sword.forward * 2f;
//         Debug.Log(sword.gameObject.name);
//     }

//     private void OnCollisionEnter(Collision collision)
//     {
//         //if (gameObject.name == "Player 2" && collision.gameObject.name == "Sword")
//             Debug.Log(gameObject.name + " COLLIDED WITH: " + collision.gameObject.name);
//     }
// >>>>>>> development
}
