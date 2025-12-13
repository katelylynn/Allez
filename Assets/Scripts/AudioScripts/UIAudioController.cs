/*
    UI Audio Controller
    Handles sound effects related to the UI.
*/

using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UIAudioController : MonoBehaviour,
    IPointerEnterHandler, IPointerDownHandler,
    ISelectHandler, ISubmitHandler
{
    [Header("Sound Clips")]
    public AudioClip cancel;
    public AudioClip confirm;
    public AudioClip cursor;

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 1f;
    public float hoverCooldown = 0.04f;
    public float clickCooldown = 0.04f;
    public float submitCooldown = 0.04f;

    private AudioSource source;
    private float nextHover;
    private float nextClick;
    private float nextSubmit;

    void Awake()
    {
        source = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D UI audio
    }

    // Mouse hover / keyboard focus
    public void OnPointerEnter(PointerEventData e) => Play(cursor, ref nextHover, hoverCooldown);
    public void OnSelect(BaseEventData e)         => Play(cursor, ref nextHover, hoverCooldown);

    // Mouse down (choose Down OR Click, not both)
    public void OnPointerDown(PointerEventData e)
    {
        // Let click take priority: clear hover gate so it can't block us
        nextHover = 0f;
        Play(confirm, ref nextClick, clickCooldown);
    }

    // Keyboard submit (e.g., Space/Enter on a focused button)
    public void OnSubmit(BaseEventData e)
    {
        nextHover = 0f;
        Play(confirm, ref nextSubmit, submitCooldown);
    }

    private void Play(AudioClip clip, ref float nextGate, float cooldown)
    {
        if (!clip) return;
        if (Time.unscaledTime < nextGate) return;
        source.PlayOneShot(clip, volume);
        nextGate = Time.unscaledTime + cooldown;
    }
}
