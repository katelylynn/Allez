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

public enum AIDifficulty
{
    Easy = 0,
    Normal = 1,
    Hard = 2,
}

public class AI : MonoBehaviour
{
    private AIDifficulty aiDifficulty;

    // references
    private Mover mover;
    private Fighter fighter;
    private GameObject opponent;
    private ScriptedMotionPlayer smp;
    private PlayerStamina stamina;

    // distance control
    public float lungeDistance = 4.6f;
    public float attackDistance = 4.0f;
    public float tolerance = 0.5f;

    // thinking
    [SerializeField] private bool isThinking;
    [SerializeField] private bool isLockedIn = false;
    public float[] guessTolerance = new float[] {
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

    // opponent
    public List<OpponentMove> opponentActionHistory;
    private Dictionary<OpponentMove, Action> actions;

    private void Start()
    {
        // easy references to the AI's own scripts
        mover = GetComponent<Mover>();
        fighter = GetComponent<Fighter>();
        smp = GetComponent<ScriptedMotionPlayer>();
        stamina = GetComponent<PlayerStamina>();

        EventManager.ActionTaken += ThinkAndReact;
        // not used (for now)
        EventManager.ActionTaken += UpdateOpponentActionHistory;

        Debug.Log(
            "AI difficulty: " + aiDifficulty +
            ", think range: " + thinkRanges[(int)aiDifficulty].x + "–" + thinkRanges[(int)aiDifficulty].y +
            ", react range: " + reactRanges[(int)aiDifficulty].x + "–" + reactRanges[(int)aiDifficulty].y
        );
    }

    public void Initialize(GameObject o, AIDifficulty aid)
    {
        // reference to the player (opponent)
        opponent = o;
        aiDifficulty = aid;
    }

    public void OnRoundReset()
    {
        StopAllCoroutines();
        isThinking = false;
        isLockedIn = false;
    }

    private void Update()
    {
        // wait until mover is active and no animations are currently running
        if (mover.enabled && !smp.isPlaying && !isLockedIn)
            ControlDistance();
    }

    private void UpdateOpponentActionHistory(OpponentMove om)
    {
        if ((om == OpponentMove.AIParried || om == OpponentMove.OpponentParried) 
            && opponentActionHistory.Count != 0
            && (opponentActionHistory[opponentActionHistory.Count - 1] == OpponentMove.AIParried 
            || opponentActionHistory[opponentActionHistory.Count - 1] == OpponentMove.OpponentParried))
            return;

        opponentActionHistory.Add(om);
    }

    private void ThinkAndReact(OpponentMove om)
    {
        // if opponent is on the ofensive...
        if (!isLockedIn
            || (om == OpponentMove.Attack || om == OpponentMove.Lunge || om == OpponentMove.AIParried || om == OpponentMove.OpponentParried) 
            && (opponentActionHistory[opponentActionHistory.Count - 1] != OpponentMove.AIParried
            && opponentActionHistory[opponentActionHistory.Count - 1] != OpponentMove.OpponentParried)
            && transform.position.z - opponent.transform.position.z <= lungeDistance + tolerance)
        {
            // Debug.Log("think and react");
            // AI thinks and then reacts!
            StartCoroutine(ThinkRoutine(() => React(om), reactRanges[(int)aiDifficulty]));
        }
    }

    private void React(OpponentMove om)
    {
        float guess = UnityEngine.Random.Range(0f, 1f);
        // Debug.Log("AI guessed: " + guess + ", " + (guess > guessTolerance[(int)aiDifficulty] ? "correctly!" : "incorrectly :( -> (threhold is " + guessTolerance[(int)aiDifficulty] + ")"));

        // if the AI successfully "guesses" the lunge...
        if (om == OpponentMove.Lunge && guess > guessTolerance[(int)aiDifficulty])
        {
            // backdash or parry
            switch (UnityEngine.Random.Range(0, 2))
            {
                case 0:
                    mover.Backdash();
                    break;
                case 1:
                    fighter.Parry(-1);
                    break;
            }
        }

        // or if the AI successfully "guesses" the attack...
        else if (om == OpponentMove.Attack && guess > guessTolerance[(int)aiDifficulty])
            fighter.Parry(-1);

        // or if the AI doesn't "guess" the attack...
        else if (om == OpponentMove.Attack && guess <= guessTolerance[(int)aiDifficulty])
        {
            // backdash (overreact) or do nothing (underreact)
            if (UnityEngine.Random.Range(0, 2) == 0)
                mover.Backdash();
        }

        // if the AI can successfully react to getting parried...
        else if (om == OpponentMove.AIParried && guess > guessTolerance[(int)aiDifficulty])
        {
            // retreat!
            isLockedIn = false;
            StopAllCoroutines();
            Debug.Log("Backdash!");
            mover.Backdash();
        }

        // if the AI can successfully react to parrying...
        else if (om == OpponentMove.OpponentParried && guess > guessTolerance[(int)aiDifficulty])
            mover.Lunge();
    }

    private void ControlDistance()
    {
        // Debug.Log("controlling distance");
        // if AI is not a good distance away from their opponent...
        if ((transform.position.z > opponent.transform.position.z + lungeDistance + tolerance || transform.position.z <= opponent.transform.position.z + lungeDistance) && !isThinking)
        {
            // move toward target distance
            mover.SetMoveAmount((transform.position.z > opponent.transform.position.z + lungeDistance + tolerance) ? 1.0f : -1.0f);
        }
        // if AI is a good range from their opponent...
        else
        {
            // stop moving and start/continue thinking loop
            mover.SetMoveAmount(0.0f);

            if (!isThinking)
                StartCoroutine(ThinkRoutine(DecideNextMove, thinkRanges[(int)aiDifficulty]));
        }
    }

    private IEnumerator ThinkRoutine(Action onFinishThinking, Vector2 range)
    {
        // Debug.Log("Starting think routine");
        isThinking = true;

        // wait a random amount of time before choosing next move
        float waitTime = UnityEngine.Random.Range(range.x, range.y);
        yield return new WaitForSeconds(waitTime);

        isThinking = false;
        onFinishThinking?.Invoke();
    }

    private void DecideNextMove()
    {
        switch (UnityEngine.Random.Range(0, 2))
        {
            case 0:
                StartCoroutine(ApproachAndAct(() => fighter.Attack(1), attackDistance));
                break;
            case 1:
                StartCoroutine(ApproachAndAct(mover.Lunge, lungeDistance));
                break;
        }
    }

    private IEnumerator ApproachAndAct(Action onFinishApproaching, float distance)
    {
        // Debug.Log("approaching");
        isLockedIn = true;

        while (transform.position.z - opponent.transform.position.z > distance)
        {
            if (opponentActionHistory.Count != 0 && opponentActionHistory[opponentActionHistory.Count-1] != OpponentMove.AIParried)
                yield break;
            // Debug.Log("in here");
            mover.SetMoveAmount(1.0f);
            yield return null; // wait for next frame
        }

        // Stop & attack
        mover.SetMoveAmount(0f);
        onFinishApproaching?.Invoke();

        isLockedIn = false;
        // Debug.Log("done approaching");
    }

    private void OnDestroy()
    {
        EventManager.ActionTaken -= ThinkAndReact;
        EventManager.ActionTaken -= UpdateOpponentActionHistory;
    }
}
