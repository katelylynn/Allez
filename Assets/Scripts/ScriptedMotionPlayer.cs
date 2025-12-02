using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ScriptedMotionPlayer : MonoBehaviour
{
    [Tooltip("Used for debug timing (expected time per phase).")]
    public float targetFPS = 60f;

    public bool isPlaying { get; private set; }

    Animator anim;
    Rigidbody rb;
    CapsuleCollider capsule;
    Coroutine currentRoutine;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }

    void OnDisable()
    {
        // If a motion is mid-play (e.g., scene reload / round reset),
        // make sure we clean up animator speed and state.
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (anim != null)
            anim.speed = 1f;

        isPlaying = false;
    }

    /// <summary>
    /// Starts a scripted motion with animation-frame syncing.
    /// direction: world direction of travel (pass Vector3.zero for no movement).
    /// </summary>
    public void PlayScriptedMotion(ScriptedMotionConfig cfg, Vector3 direction)
    {
        if (cfg == null)
        {
            Debug.LogError("[ScriptedMotionPlayer] Config is null.");
            return;
        }

        if (isPlaying)
        {
            return;
        }

        currentRoutine = StartCoroutine(ScriptedMotionRoutine(cfg, direction));
    }

    IEnumerator ScriptedMotionRoutine(ScriptedMotionConfig cfg, Vector3 direction)
    {
        isPlaying = true;
        Vector3 startPos = transform.position;

        // If direction is nearly zero, don't move the character; animation-only. This does not block other player controlled movement
        Vector3 dir = direction.sqrMagnitude > 0.0001f
            ? direction.normalized
            : Vector3.zero;

        float distance = cfg.distance;

        int startupFrames = Mathf.Max(0, cfg.startupFrames);
        int activeFrames = Mathf.Max(0, cfg.activeFrames);
        int recoveryFrames = Mathf.Max(0, cfg.recoveryFrames);
        int totalFrames = startupFrames + activeFrames + recoveryFrames;

        if (totalFrames <= 0)
        {
            //Debug.LogWarning("[ScriptedMotionPlayer] totalFrames <= 0, aborting.");
            isPlaying = false;
            yield break;
        }

        // counters for debugging
        int startupCount = 0, activeCount = 0, recoveryCount = 0;
        float startupStartTime = -1f, startupEndTime = -1f;
        float activeStartTime = -1f, activeEndTime = -1f;
        float recoveryStartTime = -1f, recoveryEndTime = -1f;

        // animation frame counts for controlling animation length
        AnimationClip clip = cfg.clip;
        int clipStartupFrames = Mathf.Max(0, cfg.startupClipFrames);
        int clipActiveFrames = Mathf.Max(0, cfg.activeClipFrames);
        int clipRecoveryFrames = Mathf.Max(0, cfg.recoveryClipFrames);
        int clipTotalFrames = clipStartupFrames + clipActiveFrames + clipRecoveryFrames;

        float normStartupStart = 0f;
        float normStartupEnd = (clipTotalFrames > 0)
            ? (float)clipStartupFrames / clipTotalFrames
            : 0.33f;
        float normActiveEnd = (clipTotalFrames > 0)
            ? (float)(clipStartupFrames + clipActiveFrames) / clipTotalFrames
            : 0.66f;
        float normRecoveryEnd = 1f;

        float originalAnimSpeed = anim.speed;

        if (clip != null) 
        {
            //anim.Update(0f);
            //anim.CrossFade(cfg.animationName, 0f, cfg.layerIndex, 0f);
            //yield return null;
            anim.speed = 0f; //must be zero initially so we can control animation times
        }

        //Debug.Log($"[ScriptedMotion] BEGIN {cfg.animationName ?? "(no clip)"} at t={Time.time:F4}, totalFrames={totalFrames}");

        for (int frameIndex = 0; frameIndex < totalFrames; frameIndex++)
        {
            float normalizedTimeForThisFrame = 0f;

            // startup portion
            if (frameIndex < startupFrames)
            {
                if (startupStartTime < 0f)
                {
                    startupStartTime = Time.time;
                    //Debug.Log($"[ScriptedMotion][Startup] START at t={startupStartTime:F4}");
                }

                startupCount++;

                float tPhase = (startupFrames <= 1)
                    ? 1f
                    : (float)frameIndex / (startupFrames - 1);

                // optional motion with animation
                if (dir != Vector3.zero && distance > 0f)
                {
                    float dist = Mathf.SmoothStep(0f, distance, tPhase);
                    Vector3 targetPos = startPos + dir * dist;
                    MoveCharacter(targetPos);
                }

                // animation normalized time
                if (clip != null)
                {
                    normalizedTimeForThisFrame = Mathf.Lerp(normStartupStart, normStartupEnd, tPhase);
                }

                if (frameIndex == startupFrames - 1)
                {
                    startupEndTime = Time.time;
                    //Debug.Log($"[ScriptedMotion][Startup] END at t={startupEndTime:F4}");
                }
            }
            // active portion
            else if (frameIndex < startupFrames + activeFrames)
            {
                if (activeStartTime < 0f)
                {
                    activeStartTime = Time.time;
                    //Debug.Log($"[ScriptedMotion][Active] START at t={activeStartTime:F4}");
                }

                activeCount++;

                int activeIndex = frameIndex - startupFrames;
                float tPhase = (activeFrames <= 1)
                    ? 1f
                    : (float)activeIndex / (activeFrames - 1);

                // hold max extension if we have motion
                if (dir != Vector3.zero && distance > 0f)
                {
                    Vector3 targetPos = startPos + dir * distance;
                    MoveCharacter(targetPos);
                }

                if (clip != null)
                {
                    float normActiveStart = normStartupEnd;
                    float normActiveEndLocal = normActiveEnd;
                    normalizedTimeForThisFrame = Mathf.Lerp(normActiveStart, normActiveEndLocal, tPhase);
                }

                if (frameIndex == startupFrames + activeFrames - 1)
                {
                    activeEndTime = Time.time;
                    //Debug.Log($"[ScriptedMotion][Active] END at t={activeEndTime:F4}");
                }
            }
            // recovery portion
            else
            {
                if (recoveryStartTime < 0f)
                {
                    recoveryStartTime = Time.time;
                    //Debug.Log($"[ScriptedMotion][Recovery] START at t={recoveryStartTime:F4}");
                }

                recoveryCount++;

                int recIndex = frameIndex - (startupFrames + activeFrames);
                float tPhase = (recoveryFrames <= 1)
                    ? 1f
                    : (float)recIndex / (recoveryFrames - 1);

                if (dir != Vector3.zero && distance > 0f)
                {
                    float startDist = cfg.returnToStart ? distance : distance;
                    float endDist = cfg.returnToStart ? 0f : distance;

                    float dist = Mathf.SmoothStep(startDist, endDist, tPhase);
                    Vector3 targetPos = startPos + dir * dist;
                    MoveCharacter(targetPos);
                }

                if (clip != null)
                {
                    float normRecStart = normActiveEnd;
                    float normRecEndLocal = normRecoveryEnd;
                    normalizedTimeForThisFrame = Mathf.Lerp(normRecStart, normRecEndLocal, tPhase);
                }

                if (frameIndex == totalFrames - 1)
                {
                    recoveryEndTime = Time.time;
                    //Debug.Log($"[ScriptedMotion][Recovery] END at t={recoveryEndTime:F4}");
                }
            }

            // normalize animation frames
            if (clip != null)
            {
                normalizedTimeForThisFrame = Mathf.Clamp01(normalizedTimeForThisFrame);
                //anim.Update(0f);
                anim.CrossFade(cfg.animationName, 0f, cfg.layerIndex, normalizedTimeForThisFrame);
                //yield return null;
            }

            // apparently works better than yield return null??
            yield return new WaitForFixedUpdate();
        }

        // snap exactly to start
        if (cfg.returnToStart && dir != Vector3.zero && distance > 0f)
        {
            MoveCharacter(startPos);
        }

        float endTime = Time.time;
        //Debug.Log($"[ScriptedMotion] END {clip?.name ?? "(no clip)"} at t={endTime:F4}");

        // debugging
        //float targetFPSLocal = targetFPS <= 0 ? 60f : targetFPS;

        //if (startupStartTime >= 0f && startupEndTime >= 0f)
        //{
        //    float elapsed = startupEndTime - startupStartTime;
        //    float expected = startupFrames / targetFPSLocal;
        //    float avgFps = startupCount / Mathf.Max(elapsed, 0.0001f);

        //    Debug.Log($"[ScriptedMotion][Startup] Frames={startupCount}/{startupFrames}, " +
        //              $"Elapsed={elapsed:F4}s, AvgFPS={avgFps:F2}, " +
        //              $"ExpectedTime@{targetFPSLocal}FPS={expected:F4}s");
        //}

        //if (activeStartTime >= 0f && activeEndTime >= 0f)
        //{
        //    float elapsed = activeEndTime - activeStartTime;
        //    float expected = activeFrames / targetFPSLocal;
        //    float avgFps = activeCount / Mathf.Max(elapsed, 0.0001f);

        //    Debug.Log($"[ScriptedMotion][Active] Frames={activeCount}/{activeFrames}, " +
        //              $"Elapsed={elapsed:F4}s, AvgFPS={avgFps:F2}, " +
        //              $"ExpectedTime@{targetFPSLocal}FPS={expected:F4}s");
        //}

        //if (recoveryStartTime >= 0f && recoveryEndTime >= 0f)
        //{
        //    float elapsed = recoveryEndTime - recoveryStartTime;
        //    float expected = recoveryFrames / targetFPSLocal;
        //    float avgFps = recoveryCount / Mathf.Max(elapsed, 0.0001f);

        //    Debug.Log($"[ScriptedMotion][Recovery] Frames={recoveryCount}/{recoveryFrames}, " +
        //              $"Elapsed={elapsed:F4}s, AvgFPS={avgFps:F2}, " +
        //              $"ExpectedTime@{targetFPSLocal}FPS={expected:F4}s");
        //}

        // restore animator speed & clear flags
        anim.speed = originalAnimSpeed;
        isPlaying = false;
        currentRoutine = null;
    }

    void MoveCharacter(Vector3 targetPos)
    {
        Vector3 currentPos = (rb != null && rb.isKinematic == false) ? rb.position : transform.position;
        Vector3 displacement = targetPos - currentPos;

        if (displacement.sqrMagnitude < 0.000001f)
        {
            if (rb != null && rb.isKinematic == false)
                rb.MovePosition(targetPos);
            else
                transform.position = targetPos;

            return;
        }

        Vector3 dir = displacement.normalized;
        float dist = displacement.magnitude;

        float castRadius = 0.3f;
        float castHeight = 1.8f;

        if (capsule != null)
        {
            castRadius = capsule.radius;
            castHeight = capsule.height;
        }

        float halfHeight = Mathf.Max(castHeight * 0.5f - castRadius, 0f);

        Vector3 center = currentPos + transform.up * (castRadius + halfHeight);
        Vector3 point1 = center + transform.up * halfHeight;
        Vector3 point2 = center - transform.up * halfHeight;

        RaycastHit hit;
        float skin = 0.02f;
        bool blocked;

        if (capsule != null)
        {
            blocked = Physics.CapsuleCast(
                point1,
                point2,
                castRadius,
                dir,
                out hit,
                dist,
                ~0,
                QueryTriggerInteraction.Ignore
            );
        }
        else
        {
            blocked = Physics.SphereCast(
                currentPos,
                castRadius,
                dir,
                out hit,
                dist,
                ~0,
                QueryTriggerInteraction.Ignore
            );
        }

        if (blocked && hit.distance > 0f)
        {
            float allowed = Mathf.Max(hit.distance - skin, 0f);
            targetPos = currentPos + dir * allowed;
        }

        if (rb != null && rb.isKinematic == false)
        {
            rb.MovePosition(targetPos);
        }
        else
        {
            transform.position = targetPos;
        }
    }

    public void StopCurrentMotion()
    {
        if (currentRoutine != null)
        {
            Debug.Log("Stopping current routine");
            StopCoroutine(currentRoutine);
            anim.speed = 1f;
            anim.CrossFade(ScriptedMotionConfig.interruptStateName, 0f, ScriptedMotionConfig.interruptLayerIndex, 0f);
            anim.Update(0f);
            isPlaying = false;
            currentRoutine = null;
        }
    }
}
