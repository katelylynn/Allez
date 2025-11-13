using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{
    // references
    private Mover mover;
    private Fighter fighter;
    private GameObject opponent;

    // distance control
    private float distance;
    public float targetDistance = 4.6f;
    public float tolerance = 0.5f;

    // thinking
    public float minThink = 0.2f;
    public float maxThink = 1.0f;
    private bool isThinking;

    private void Start()
    {
        // easy references to the AI's own scripts
        mover = GetComponent<Mover>();
        fighter = GetComponent<Fighter>();
    }

    public void Initialize(GameObject o)
    {
        // reference to the player (opponent)
        opponent = o;
    }

    private void Update()
    {
        // wait until mover is active
        if (mover.enabled)
            ControlDistance();
    }

    private void ControlDistance()
    {
        distance = transform.position.z - opponent.transform.position.z;

        // if AI is not a good distance away from their opponent
        if (Mathf.Abs(distance - targetDistance) > tolerance)
        {
            // move toward target distance
            mover.SetMoveAmount((distance - targetDistance > 0) ? 1.0f : -1.0f);

            // stop thinking loop if we leave the target range
            if (isThinking)
            {
                StopAllCoroutines();
                isThinking = false;
            }
        }
        else
        {
            // we're in the target range — stop moving and start/continue thinking loop
            mover.SetMoveAmount(0.0f);

            if (!isThinking)
                StartCoroutine(ThinkRoutine());
        }
    }

    private IEnumerator ThinkRoutine()
    {
        isThinking = true;

        // Keep choosing actions while we remain in range
        while (InGoodRange())
        {
            // wait a random amount of time before choosing next move
            float waitTime = Random.Range(minThink, maxThink);
            yield return new WaitForSeconds(waitTime);

            // re-check before acting
            if (!InGoodRange()) 
                break;

            CalculateNextMove();
        }

        isThinking = false;
    }

    private bool InGoodRange()
    {
        return Mathf.Abs(transform.position.z - opponent.transform.position.z - targetDistance) <= tolerance;
    }

    private void CalculateNextMove()
    {
        switch (Random.Range(0, 5))
        {
            case 0:
                fighter.Attack();
                break;
            case 1:
                fighter.TiltLeft();
                break;
            case 2:
                fighter.TiltRight();
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
