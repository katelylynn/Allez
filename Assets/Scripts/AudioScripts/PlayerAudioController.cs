using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioController : MonoBehaviour
{
    [Header("Sound Clips")]
    public AudioClip Jump;
    public AudioClip Hit;
    public AudioClip Step;
    public AudioClip Lunge;
    public AudioClip Parry;
    public AudioClip Swing;

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        // For 3D sounds, spatialBlen = 1f and delete dopperLevel
        source.spatialBlend = 0f; // 2d sound
        source.dopplerLevel = 0f;

    }

    public void PlayJump() => Play(Jump);
    public void PlayAttack() => Play(Hit);
    public void PlayStep() => Play(Step);
    public void PlayLunge() => Play(Lunge);
    public void PlayParry() => Play(Parry);
    public void PlaySwing() => Play(Swing);

    private void Play(AudioClip clip, float volume = 1f)
    {
        if (clip != null) source.PlayOneShot(clip, volume);
    }
}
