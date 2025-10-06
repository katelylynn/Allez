using UnityEngine;

public class S_A_FencingMoveController : MonoBehaviour
{
    public KeyCode lungeKey = KeyCode.LeftShift;
    public KeyCode parryKey = KeyCode.LeftControl;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.Log("No animator found!");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(lungeKey)) {
            animator.SetTrigger("LungeCenter");
        } else if (Input.GetKeyDown(parryKey)) {
            animator.SetTrigger("ParryLeft");
        }
    }
}
