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
        CalculateNextMove();
    }

    private void CalculateNextMove()
    {
        Debug.Log(CheckIfGoodDistance());
    }

    private bool CheckIfGoodDistance()
    {
        return (Mathf.Abs(transform.position.z - opponent.transform.position.z - targetDistance) < tolerance);
    }
}
