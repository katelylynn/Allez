using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int rounds = 3;
    public int currRound = 0;
    public int[] score = {0, 0};

    private void Start()
    {
        EventManager.RoundEnd += EndRound;
    }

    public void StartDuel()
    {
        Countdown();
        EventManager.TriggerRoundStart();
    }

    private void Countdown()
    {
        Debug.Log("En garde, pret, allez!");
    }

    private void EndRound(int winner)
    {
        score[winner]++;
        Debug.Log("hit scored! winner: fencer " + winner);
        Debug.Log("round: " + currRound + ", score: " + score[0] + ", " + score[1]);
    }
}
