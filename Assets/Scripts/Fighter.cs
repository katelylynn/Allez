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
        if (tiltDirection.Get<float>() == -1)
        {
            anim.SetTrigger("ParryLeft");
            StartCoroutine(DoParry(leftParryPos));
        }
        else if (tiltDirection.Get<float>() == 1)
        {
            anim.SetTrigger("ParryRight");
            StartCoroutine(DoParry(rightParryPos));
        }
        else
        {
            // When player releases (unholds) the parry key, return arm to OG position
            StartCoroutine(DoParry(unParryPos));
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
