using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    private Animator anim;

    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
    }

    public void OnAttack(InputValue value)
    {
        Attack();
    }

    public void TiltLeft()
    {
        anim.SetTrigger("ParryLeft");
    }

    public void TiltRight()
    {
        anim.SetTrigger("ParryRight");
    }

    public void OnTilt(InputValue tiltDirection)
    {
        if (tiltDirection.Get<float>() == -1)
        {
            TiltLeft();
        }
        else if (tiltDirection.Get<float>() == 1)
        {
            TiltRight();
        }
    }
}
