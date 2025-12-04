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
    private PlayerStamina stamina;

    // distance control
    public float lungeDistance = 4.6f;
    public float attackDistance = 4.0f;
    public float tolerance = 0.5f;

    // thinking
    [SerializeField] private bool isThinking;
    public float[] thinkRange = new float[] { 0.2f, 0.5f };
    public float[] reactRange = new float[] { 0f, 0.3f };
    public float guessTolerance = 0.4f;
    [SerializeField] private bool isLockedIn = false;

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
            || (om == OpponentMove.Attack || om == OpponentMove.Lunge) 
            && transform.position.z - opponent.transform.position.z <= lungeDistance + tolerance)
            // AI thinks and then reacts!
            StartCoroutine(ThinkRoutine(() => React(om), reactRange));
    }

    private void React(OpponentMove om)
    {
        float guess = UnityEngine.Random.Range(0f, 1f);
        Debug.Log("AI guessed: " + guess + ", " + (guess > guessTolerance ? "correctly!" : "incorrectly :("));

        // if the AI successfully "guesses" the lunge...
        if (om == OpponentMove.Lunge && guess > guessTolerance)
            // backdash out of the way
            mover.Backdash();

        // or if the AI successfully "guesses" the attack...
        else if (om == OpponentMove.Attack && guess > guessTolerance)
            fighter.Parry(-1);

        // or if the AI doesn't "guess" the attack...
        else if (om == OpponentMove.Attack && guess <= guessTolerance)
            mover.Backdash();
    }

    private void ControlDistance()
    {
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
                StartCoroutine(ThinkRoutine(DecideNextMove, thinkRange));
        }
    }

    private IEnumerator ThinkRoutine(Action onFinishThinking, float[] range)
    {
        isThinking = true;

        // wait a random amount of time before choosing next move
        float waitTime = UnityEngine.Random.Range(range[0], range[1]);
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
        isLockedIn = true;

        while (transform.position.z - opponent.transform.position.z > distance)
        {
            mover.SetMoveAmount(1.0f);
            yield return null; // wait for next frame
        }

        // Stop & attack
        mover.SetMoveAmount(0f);
        onFinishApproaching?.Invoke();

        isLockedIn = false;
    }

    private void OnDestroy()
    {
        EventManager.ActionTaken -= ThinkAndReact;
        EventManager.ActionTaken -= UpdateOpponentActionHistory;
    }
}
