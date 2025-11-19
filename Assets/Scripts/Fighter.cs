using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    public Transform ParryTracker;
    public float tiltSpeed = 5;
    public float parryForce = 3;

    private Animator anim;

    private float leftTiltPos = -5;
    private float rightTiltPos = 2;
    private float unTiltPos = 0;

    private Coroutine currentTiltCoroutine;
    private Coroutine currentParryCoroutine;

    private bool isTilting = false;

    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnAttack(InputValue value)
    {
        if (!anim.GetBool("Parry"))
        anim.SetTrigger("Attack");
    }

    public void OnTilt(InputValue tiltDirection)
    {
        float tilt = tiltDirection.Get<float>();

        if (currentTiltCoroutine != null)
            StopCoroutine(currentTiltCoroutine);

        if (tilt == -1)
        {
            currentTiltCoroutine = StartCoroutine(DoTilt(leftTiltPos));
        }
        else if (tilt == 1)
        {
            currentTiltCoroutine = StartCoroutine(DoTilt(rightTiltPos));
        }
        else
        {
            currentTiltCoroutine = StartCoroutine(DoTilt(unTiltPos));
        }
    }

    private IEnumerator DoTilt(float targetLocalX)
    {
        float time = 0;
        Vector3 startPos = ParryTracker.localPosition;
        Vector3 targetPos = new Vector3(targetLocalX, startPos.y, startPos.z);
        isTilting = true;

        while (time < 1)
        {
            ParryTracker.localPosition = Vector3.Lerp(startPos, targetPos, time);
            time += Time.deltaTime * tiltSpeed;
            yield return null;
        }

        isTilting = false;
        ParryTracker.localPosition = targetPos; // Snap to final position
    }

    public void OnParry(InputValue parryDirection)
    {
        // do the parry action
        if (currentParryCoroutine == null && !isTilting)
        {
            float parryDir = parryDirection.Get<float>();
            GameObject Rig1 = ParryTracker.parent.gameObject;
            //Rig1.transform.GetChild(0).gameObject.SetActive(false);
            //Rig1.transform.GetChild(3).gameObject.SetActive(true);

            if (parryDir == -1)
            {
                //parry left
                currentParryCoroutine = StartCoroutine(DoParry(-parryForce, true));
            }
            else if (parryDir == 1)
            {
                //parry right
                currentParryCoroutine = StartCoroutine(DoParry(parryForce, true));
            }
        }
    }

    private IEnumerator DoParry(float direction, bool isReversing = false)
    {
        anim.SetBool("Parry", true);
        float time = 0;
        Vector3 startPos = ParryTracker.localPosition;

        // Instead of absolute position, use relative offset:
        Vector3 targetPos = startPos + new Vector3(direction, 0, 0);

        while (time < 1)
        {
            ParryTracker.localPosition = Vector3.Lerp(startPos, targetPos, time);
            time += Time.deltaTime * tiltSpeed;
            yield return null;
        }

        ParryTracker.localPosition = targetPos;
        GameObject Rig1 = ParryTracker.parent.gameObject;
        //Rig1.transform.GetChild(0).gameObject.SetActive(true);
        //Rig1.transform.GetChild(3).gameObject.SetActive(false);
        currentParryCoroutine = null;

        if (isReversing)
            currentParryCoroutine = StartCoroutine(DoParry(-direction));
        
        anim.SetBool("Parry", false);
    }
}
