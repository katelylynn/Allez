using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioController : MonoBehaviour
{
    [Header("Sound Clips")]
    public AudioClip Hit;
    public AudioClip Step;
    public AudioClip Lunge;
    public AudioClip Parry;
    public AudioClip Swing;
    public AudioClip BackDash;
    public AudioClip Ouch;

    [Header("Female Sound Clips")]
    public AudioClip LungeFemale;
    public AudioClip OuchFemale;
    public AudioClip BackDashFemale;
    
    [Header("Male Sound Clips")]
    public AudioClip LungeMale;
    public AudioClip OuchMale;
    public AudioClip BackDashMale;

    private AudioSource source;

    public void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        // source.spatialBlend = 0f; 
        // source.dopplerLevel = 0f;
    }

    public void SetGenderAudios(FencerId id)
    {
        // Female
        if (id == FencerId.Fencer0)   
        {
            Lunge = LungeFemale;
            Ouch = OuchFemale;
            BackDash = BackDashFemale;
        }
        // Male
        else if (id == FencerId.Fencer1) 
        {
            Lunge = LungeMale;
            Ouch = OuchMale;
            BackDash = BackDashMale;
        }
    }

    public void PlayAttack() => Play(Hit);
    public void PlayStep() => Play(Step);
    public void PlayLunge() => Play(Lunge);
    public void PlayParry() => Play(Parry);
    public void PlaySwing() => Play(Swing);
    public void PlayOuchOne() => Play(BackDash);
    public void PlayOuchTwo2() => Play(Ouch);

    private void Play(AudioClip clip, float volume = 1f)
    {
        if (clip != null) source.PlayOneShot(clip, volume);
    }
}
