using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class EventManager
{
    // ROUND EVENTS
    public static event Action RoundStart;
    public static event Action<FencerId> RoundEnd;

    // COMBAT EVENTS
    public static event Action ParrySuccess;

    // TRIGGERS
    public static void TriggerRoundStart() 
    {
        RoundStart?.Invoke();
        
    }

    public static void TriggerRoundEnd(FencerId winner) 
    {
        RoundEnd?.Invoke(winner);
    }

    public static void TriggerParrySuccess()
    {
        ParrySuccess?.Invoke();
    }
}