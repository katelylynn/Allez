using UnityEngine;

[System.Serializable]
public class ScriptedMotionConfig
{
    [Header("Animation name in Animator. Must match exactly")]
    public string animationName = "";

    [Header("Animation")]
    public AnimationClip clip;
    public int layerIndex = 0;

    [Tooltip("How far along the motion direction this move travels.")]
    public float distance = 2f;

    [Tooltip("If true, recovery returns to start position. If false, stays at max distance.")]
    public bool returnToStart = true;

    [Header("Game 'frame' counts (startup/active/recovery)")]
    public int startupFrames = 25;
    public int activeFrames = 5;
    public int recoveryFrames = 40;

    [Header("Animation clip frame counts\n!!!Make sure these add up to the total frame count of the original animation!!!")]
    public int startupClipFrames = 20;
    public int activeClipFrames = 10;
    public int recoveryClipFrames = 30;

    [Header("Stamina")]
    public int staminaCost = 0;
}
