using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    private Animator anim;

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
        }
        else if (tiltDirection.Get<float>() == 1)
        {
            anim.SetTrigger("ParryRight");
        }
    }
}
