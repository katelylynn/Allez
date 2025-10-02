using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int rounds = 3;
    public int currRound = 0;
    public (int, int) score = (0, 0);

    public void StartDuel()
    {
        Countdown();
        EventManager.TriggerRoundStart();
    }

    private void Countdown()
    {
        Debug.Log("En garde, pret, allez!");
    }
}
