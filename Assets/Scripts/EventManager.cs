using System;
using UnityEngine;

public static class EventManager
{
    public static event Action RoundStart;
    public static event Action<int> RoundEnd;

    public static void TriggerRoundStart() 
    {
        RoundStart?.Invoke();
    }

    public static void TriggerRoundEnd(int winner) 
    {
        RoundEnd?.Invoke(winner);
    }
}