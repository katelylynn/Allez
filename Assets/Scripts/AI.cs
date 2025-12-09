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
    AIParried, // opponent parries the AI
    OpponentParried, // AI parries the opponent
}

public enum AIDifficulty
{
    Easy = 0,
    Normal = 1,
    Hard = 2,
}

public class AI : MonoBehaviour
{
    // AI params
    private AIDifficulty aiDifficulty;
    [SerializeField] private Coroutine currentRoutine;

    // references
    private Mover mover;
    private Fighter fighter;
    private GameObject opponent;
    private ScriptedMotionPlayer smp;
    private PlayerStamina stamina;

    // opponent
    public List<OpponentMove> opponentActionHistory;
    private Dictionary<OpponentMove, Action> actions;

    // distance control
    public float lungeDistance = 7f;
    public float attackDistance = 4f;
    public float tolerance = 0.5f;

    // intervals
    public float[] reactionThresholds = new float[] {
        0.7f, // easy
        0.4f, // normal
        0f, // hard
    };
    public Vector2[] thinkRanges = new Vector2[]
    {
        new Vector2(0.2f, 0.6f), // easy
        new Vector2(0.2f, 0.4f), // normal
        new Vector2(0f, 0.2f), // hard
    };
    public Vector2[] reactRanges = new Vector2[]
    {
        new Vector2(0.1f, 0.5f), // easy
        new Vector2(0f, 0.3f), // normal
        new Vector2(0f, 0.1f), // hard
    };

    private void Start()
    {
        // easy references to the AI's own scripts
        mover = GetComponent<Mover>();
        fighter = GetComponent<Fighter>();
        smp = GetComponent<ScriptedMotionPlayer>();
        stamina = GetComponent<PlayerStamina>();

        // subscribe to events
        EventManager.RoundReset += OnRoundReset;
        EventManager.ActionTaken += UpdateOpponentActionHistory;
        EventManager.ActionTaken += ThinkAndReact;

        Debug.Log(
            "AI difficulty: " + aiDifficulty +
            ", think range: " + thinkRanges[(int)aiDifficulty].x + "–" + thinkRanges[(int)aiDifficulty].y +
            ", react range: " + reactRanges[(int)aiDifficulty].x + "–" + reactRanges[(int)aiDifficulty].y
        );
    }

    private void OnDestroy()
    {
        EventManager.RoundReset -= OnRoundReset;
        EventManager.ActionTaken -= UpdateOpponentActionHistory;
        EventManager.ActionTaken -= ThinkAndReact;
    }

    public void Initialize(GameObject o, AIDifficulty aid)
    {
        opponent = o;
        aiDifficulty = aid;
    }

    public void OnRoundReset()
    {
        mover.SetMoveAmount(0f);
        if (currentRoutine != null)
        {
            Debug.Log("AI: shutting down current routine");
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }

    private void UpdateOpponentActionHistory(OpponentMove om)
    {
        if (
            // if history is not empty...
            opponentActionHistory.Count != 0
            && (
                // check whether this is the duplicated "parried" move (parried fires twice)
                (om == OpponentMove.AIParried && opponentActionHistory[opponentActionHistory.Count - 1] == OpponentMove.AIParried)
                || (om == OpponentMove.AIParried && opponentActionHistory[opponentActionHistory.Count - 1] == OpponentMove.AIParried)
            )
        )
            // ignore this duplicated move
            return;

        // otherwise, add to history
        opponentActionHistory.Add(om);
    }

    private void ThinkAndReact(OpponentMove om)
    {
        if (
            // if the opponent attacks/lunges or a "parried" occurs...
            (om == OpponentMove.Attack || om == OpponentMove.Lunge || om == OpponentMove.AIParried || om == OpponentMove.OpponentParried)
            // and the AI is in striking range...
            && transform.position.z - opponent.transform.position.z <= lungeDistance + tolerance
        )
        {
            // AI stops what it's currently doing
            if (currentRoutine != null)
            {
                StopCoroutine(currentRoutine);
                currentRoutine = null;
            }

            // AI thinks and reacts
            if (aiDifficulty == AIDifficulty.Easy || aiDifficulty == AIDifficulty.Normal)
                currentRoutine = StartCoroutine(ThinkRoutine(() => React(om), reactRanges[(int)aiDifficulty]));
            else
                React(om);
        }
    }

    private IEnumerator ThinkRoutine(Action onFinishThinking, Vector2 range)
    {
        Debug.Log("AI: thinking...");

        // wait a random amount of time to simulate thinking
        float waitTime = UnityEngine.Random.Range(range.x, range.y);
        yield return new WaitForSeconds(waitTime);

        // mark this routine as complete
        currentRoutine = null;

        // act
        onFinishThinking?.Invoke();
    }

    private void React(OpponentMove opponentMove)
    {
        float reaction = UnityEngine.Random.Range(0f, 1f);
        Debug.Log("AI reacts " + (reaction > reactionThresholds[(int)aiDifficulty] ? "successfully :)" : "unsuccessfully :("));

        // if AI reacts in time...
        if (reaction > reactionThresholds[(int)aiDifficulty])
        {
            switch (opponentMove)
            {
                // and opponent is lunging...
                case OpponentMove.Lunge:
                    // backdash or parry!
                    if (UnityEngine.Random.Range(0, 2) == 0) mover.Backdash();
                    else fighter.Parry(-1);
                    break;
                // and opponent is attacking...
                case OpponentMove.Attack:
                    // parry!
                    fighter.Parry(-1);
                    break;
                // and opponent successfully parries AI...
                case OpponentMove.AIParried:
                    // run!
                    mover.Backdash();
                    break;
                // and AI successfully parries opponent...
                case OpponentMove.OpponentParried:
                    // go in for the kill
                    DecideNextMove();
                    break;
            }
        }
        // if the AI doesn't react in time...
        else
        {
            switch (opponentMove)
            {
                // and opponent is attacking...
                case OpponentMove.Attack:
                    // backdash (overreact) or do nothing (underreact)
                    if (UnityEngine.Random.Range(0, 2) == 0) mover.Backdash();
                    break;
            }
        }
    }

    private void Update()
    {
        if (
            // if moving is allowed...
            mover.enabled
            // and the AI isn't currently doing anything...
            && currentRoutine == null
        )
            ControlDistance();
    }

    private void ControlDistance()
    {
        // Debug.Log("AI: controlling distance");

        if (
            // if AI is too far from opponent...
            transform.position.z > opponent.transform.position.z + lungeDistance + tolerance 
            // or too close too opponent...
            || transform.position.z <= opponent.transform.position.z + lungeDistance
        )
        {
            // move toward target distance
            // Debug.Log("AI: moving toward target distance");
            mover.SetMoveAmount((transform.position.z > opponent.transform.position.z + lungeDistance + tolerance) ? 1.0f : -1.0f);
        }
        // if AI is a good range from their opponent...
        else
        {
            // stop moving
            Debug.Log("AI: at target distance");
            mover.SetMoveAmount(0.0f);

            // think and execute offensive attack!
            if (aiDifficulty == AIDifficulty.Easy || aiDifficulty == AIDifficulty.Normal)
                currentRoutine = StartCoroutine(ThinkRoutine(DecideNextMove, thinkRanges[(int)aiDifficulty]));
            else
                DecideNextMove();
        }
    }

    private void DecideNextMove()
    {
        // either...
        if (UnityEngine.Random.Range(0, 2) == 0)
            // attack
            currentRoutine = StartCoroutine(ApproachAndAct(() => fighter.Attack(1), attackDistance));
        else
            // lunge
            currentRoutine = StartCoroutine(ApproachAndAct(mover.Lunge, lungeDistance));
    }

    private IEnumerator ApproachAndAct(Action onFinishApproaching, float distance)
    {
        // while out of range for the desired attack... 
        while (transform.position.z - opponent.transform.position.z > distance)
        {
            // approach
            // Debug.Log("AI: approaching opponent to attack");
            mover.SetMoveAmount(1.0f);
            yield return null; // wait for next frame
        }

        // stop and attack
        Debug.Log("AI: attack!");
        onFinishApproaching?.Invoke();

        // mark this routine as complete
        currentRoutine = null;
    }
}
