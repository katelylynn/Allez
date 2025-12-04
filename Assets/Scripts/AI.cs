using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum OpponentMove
{
    Attack,
    Parry,
    Lunge,
    Backdash,
    AIParried,
    OpponentParried,
}

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

    // opponent
    private Dictionary<OpponentMove, Action> actions;
    public List<OpponentMove> opponentActionHistory;

    private void Start()
    {
        // easy references to the AI's own scripts
        mover = GetComponent<Mover>();
        fighter = GetComponent<Fighter>();
        smp = GetComponent<ScriptedMotionPlayer>();

        // setup possible actions
        actions = new Dictionary<OpponentMove, Action>
        {
            { OpponentMove.Attack,   () => fighter.Attack(1) },
            { OpponentMove.Parry,    () => fighter.Parry(-1) },
            { OpponentMove.Lunge,    mover.Lunge },
            { OpponentMove.Backdash, mover.Backdash }
        };
        EventManager.ActionTaken += UpdateOpponentActionHistory;
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

    private void UpdateOpponentActionHistory(OpponentMove om)
    {
        if ((om == OpponentMove.AIParried || om == OpponentMove.OpponentParried) 
            && opponentActionHistory.Count != 0
            && (opponentActionHistory[opponentActionHistory.Count - 1] == OpponentMove.AIParried 
            || opponentActionHistory[opponentActionHistory.Count - 1] == OpponentMove.OpponentParried))
        {
            Debug.Log(om == OpponentMove.AIParried || om == OpponentMove.OpponentParried);
            Debug.Log(opponentActionHistory.Count != 0);
            Debug.Log(opponentActionHistory[opponentActionHistory.Count - 1] != OpponentMove.AIParried);
            Debug.Log(opponentActionHistory[opponentActionHistory.Count - 1] != OpponentMove.OpponentParried);
            return;
        }

        opponentActionHistory.Add(om);
    }

    private void ControlDistance()
    {
        if (smp.isPlaying)
        {
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
        float waitTime = UnityEngine.Random.Range(minThink, maxThink);
        yield return new WaitForSeconds(waitTime);

        isThinking = false;
        CalculateNextMove();
    }

    private void CalculateNextMove()
    {
        /*
        Possible moves:
            fighter.Attack(1);
            fighter.Parry(-1);
            mover.Lunge();
            mover.Backdash();
        */
    }
}
