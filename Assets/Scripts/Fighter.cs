using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    private Animator anim;
    public GameObject foilAttackBox;
    //public bool foilHitBoxEnabled = true;
    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnValidate()
    {
        //foilAttackBox.SetActive(foilHitBoxEnabled);
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
