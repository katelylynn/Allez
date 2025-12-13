/*
    Score Audio Controller
    Controls audio that happens on point scored.
*/

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerScore : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip p1Ouch;
    public AudioClip p2Ouch;
    public AudioClip cheers;

    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.dopplerLevel = 0f;
    }

    private void OnEnable()
    {
        EventManager.RoundEnd += OnRoundEnd;
    }

    private void OnDisable()
    {
        EventManager.RoundEnd -= OnRoundEnd;
    }

    private void OnRoundEnd(FencerId winner)
    {
        switch (winner)
        {
            case FencerId.Fencer0: // P1 scored
                Play(p2Ouch);
                break;

            case FencerId.Fencer1: // P2 scored
                Play(p1Ouch);
                break;
        }

        Play(cheers);
    }

    private void Play(AudioClip clip)
    {
        if (clip != null)
            source.PlayOneShot(clip, sfxVolume);
    }
}
