using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{
    // references
    private Mover mover;
    private Fighter fighter;
    private GameObject opponent;
    private ScriptedMotionPlayer smp;

    // distance control
    private float distance;
    public float targetDistance = 4.6f;
    public float tolerance = 0.5f;

    // thinking
    public float minThink = 0.2f;
    public float maxThink = 1.0f;
    [SerializeField] private bool isThinking;

    private void Start()
    {
        // easy references to the AI's own scripts
        mover = GetComponent<Mover>();
        fighter = GetComponent<Fighter>();
        smp = GetComponent<ScriptedMotionPlayer>();
    }

    public void Initialize(GameObject o)
    {
        // reference to the player (opponent)
        opponent = o;
    }

    public void OnRoundReset()
    {
        StopAllCoroutines();
        isThinking = false;
    }

    private void Update()
    {
        // wait until mover is active
        if (mover.enabled)
            ControlDistance();
    }

    private void ControlDistance()
    {
        if (smp.isPlaying)
        {
            Debug.Log("reached");
            mover.SetMoveAmount(0f);
            return;
        }

        distance = transform.position.z - opponent.transform.position.z;

        // if AI is not a good distance away from their opponent...
        if (Mathf.Abs(distance - targetDistance) > tolerance && !isThinking)
        {
            // move toward target distance
            mover.SetMoveAmount((distance - targetDistance > 0) ? 1.0f : -1.0f);
        }
        // if AI is a good range from their opponent...
        else
        {
            // stop moving and start/continue thinking loop
            mover.SetMoveAmount(0.0f);

            if (!isThinking)
                StartCoroutine(ThinkRoutine());
        }
    }

    private IEnumerator ThinkRoutine()
    {
        isThinking = true;

        // wait a random amount of time before choosing next move
        float waitTime = Random.Range(minThink, maxThink);
        yield return new WaitForSeconds(waitTime);

        isThinking = false;
        CalculateNextMove();
    }

    private void CalculateNextMove()
    {
        switch (Random.Range(0, 5))
        {
            case 0:
                fighter.Attack(1);
                break;
            case 2:
                fighter.Parry(-1);
                break;
            case 3:
                mover.Lunge();
                break;
            case 4:
                mover.Backdash();
                break;
        }
    }
}
