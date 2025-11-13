using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    public Transform ParryTracker;
    public float parrySpeed = 5;

    private Animator anim;
    private float leftParryPos = -2;
    private float rightParryPos = 2;
    private float unParryPos = 0;
    private Coroutine currentParryCoroutine;

    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnAttack(InputValue value)
    {
        anim.SetTrigger("Attack");
    }

    public void OnTilt(InputValue tiltDirection)
    {
        float tilt = tiltDirection.Get<float>();

        if (currentParryCoroutine != null)
            StopCoroutine(currentParryCoroutine);

        if (tilt == -1)
        {
            anim.SetTrigger("ParryLeft");
            currentParryCoroutine = StartCoroutine(DoParry(leftParryPos));
        }
        else if (tilt == 1)
        {
            anim.SetTrigger("ParryRight");
            currentParryCoroutine = StartCoroutine(DoParry(rightParryPos));
        }
        else
        {
            currentParryCoroutine = StartCoroutine(DoParry(unParryPos));
        }
    }

    private IEnumerator DoParry(float targetLocalX)
    {
        float time = 0;
        Vector3 startPos = ParryTracker.localPosition;
        Vector3 targetPos = new Vector3(targetLocalX, startPos.y, startPos.z);

        while (time < 1)
        {
            ParryTracker.localPosition = Vector3.Lerp(startPos, targetPos, time);
            time += Time.deltaTime * parrySpeed;
            yield return null;
        }

        ParryTracker.localPosition = targetPos; // Snap to final position
    }
}
