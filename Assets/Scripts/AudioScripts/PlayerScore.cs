using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerScore : MonoBehaviour
{

    [Header("SFX")]
    public AudioClip p1PointSfx;
    public AudioClip p2PointSfx;
    public AudioClip p1Ouch;
    public AudioClip p2Ouch;

    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource source;

    // cached previous scores
    private int prevP1;
    private int prevP2;


    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D UI-style
        source.dopplerLevel = 0f;

        // initialize from current PlayerPrefs so we don't fire on scene load
        prevP1 = PlayerPrefs.GetInt("P1Score");
        prevP2 = PlayerPrefs.GetInt("P2Score");
    }

    void Update()
    {
        int p1 = PlayerPrefs.GetInt("P1Score");
        int p2 = PlayerPrefs.GetInt("P2Score");

        // point SFX (only when score increases)
        if (p1 > prevP1) {             
            // Play(p2Ouch);
            Play(p1PointSfx); 
        }


        if (p2 > prevP2)
        {
            // Play(p1Ouch);
            Play(p2PointSfx);
        }

        // update previous after checks
        prevP1 = p1;
        prevP2 = p2;
    }

    public void ResetMatchState()
    {
        prevP1 = PlayerPrefs.GetInt("P1Score");
        prevP2 = PlayerPrefs.GetInt("P2Score");
    }

    private void Play(AudioClip clip)
    {
        if (clip != null)
            source.PlayOneShot(clip, sfxVolume);
    }
}
