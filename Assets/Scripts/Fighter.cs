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
        Debug.Log("Attacking");
    }

    public void OnTilt(InputValue tiltDirection)
    {
        if (tiltDirection.Get<float>() == -1)
        {
            anim.SetTrigger("ParryLeft");
        }
        else if (tiltDirection.Get<float>() == 1)
        {
            Debug.Log("Parry Right");
            // anim.SetTrigger("ParryRight");
        }
    }
}
