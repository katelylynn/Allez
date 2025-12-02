using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class EventManager
{
    // ROUND EVENTS
    public static event Action RoundStart;
    public static event Action<FencerId> RoundEnd;
    public static event Action RoundReset;
    public static event Action<bool> InputEnable;
    // COMBAT EVENTS
    public static event Action ParrySuccess;
    // UI EVENTS
    public static event Action Pause;

    // TRIGGERS
    public static void TriggerRoundStart() 
    {
        RoundStart?.Invoke();
    }

    public static void TriggerRoundEnd(FencerId winner) 
    {
        RoundEnd?.Invoke(winner);
    }

    public static void TriggerRoundReset()
    {
        RoundReset?.Invoke();
    }

    public static void TriggerInputEnable(bool enable)
    {
        InputEnable?.Invoke(enable);
    }

    public static void TriggerParrySuccess()
    {
        ParrySuccess?.Invoke();
    }

    public static void TriggerPause()
    {
        Pause?.Invoke();
    }
}