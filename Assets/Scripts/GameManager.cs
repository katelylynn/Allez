using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currRound = 1;
    public int[] score = {0, 0};
    public int pointsToWin = 3;

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

        if (score[0] == pointsToWin || score[1] == pointsToWin)
        {
            EndFight();
        }
        else
        {
            currRound++;
            EventManager.TriggerRoundStart();
        }
    }

    private void EndFight()
    {
        Debug.Log("GAME OVER!");
        Debug.Log("winner: fencer " + ((score[0] == pointsToWin) ? "0" : "1"));
    }
}
