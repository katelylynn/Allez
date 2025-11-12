using UnityEngine;

public class AI : MonoBehaviour
{
    private Mover mover;
    private Fighter fighter;
    private GameObject opponent;

    private float distance;
    public float targetDistance = 4.6f;
    public float tolerance = 0.5f;

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
            // get the AI to move in the direction of a good distance
            mover.SetMoveAmount((distance - targetDistance > 0) ? 1.0f : -1.0f);
        }
        else
        {
            mover.SetMoveAmount(0.0f);
        }
    }
}
