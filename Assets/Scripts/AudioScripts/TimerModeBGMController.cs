using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TimerModeBGMController : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Sound Clips")]
    public AudioClip TimerBGM;
    // public AudioClip ScoreBGM;

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();

        if (source == null)
        {
            Debug.LogError($"[BGM] No AudioSource found on {name}");
            return;
        }

        source.playOnAwake = true;
        source.loop = true;

        // gameManager = FindObjectOfType<GameManager>();
    }

    // Called by GameManager.Initialize()
    // public void OnGameManagerReady(GameMode gm)
    // {
    //     // Debug.Log("BGM Controller: GameManager is ready! Starting BGM...");

    //     // Debug.Log($"BGM GameMode from GM: {gm}");

    //     // if (gm == GameMode.MostPointsInXTime)
    //     //     PlayLoop(TimerBGM, 1f, 1f);
    //     // else
    //     //     PlayLoop(ScoreBGM, 1f, 1f);
    // }

    void PlayLoop(AudioClip clip, float volume, float pitch)
    {
        // if (clip == null)
        // {
        //     Debug.LogWarning("[BGM] Missing clip!");
        //     return;
        // }
        // if (source == null) return;

        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }

    void Update()
    {
        if (gameManager == null || source == null) return;

        int timer = (int)gameManager.elapsedTime;

        if (timer <= 30)
            source.pitch = 1.0f;
        else if (timer <= 60)
            source.pitch = 1.1f;
        else if (timer <= 90)
            source.pitch = 1.2f;
        else 
            source.pitch = 1.3f;
    }
}
