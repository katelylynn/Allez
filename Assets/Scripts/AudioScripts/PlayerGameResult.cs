using UnityEngine;
using TMPro;

public class PlayerGameResult : MonoBehaviour
{
    [Header("Sound Clips")]
    // public AudioClip P1Win;
    // public AudioClip P2Win;
    public AudioClip backgroundMusic;
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
    }

    private void Play(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
            source.PlayOneShot(clip, volume);
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            source.clip = backgroundMusic;
            source.loop = true;
            source.volume = 1f;
            source.Play();
        }
    }

    void Start()
    {
        // Start background music
        PlayBackgroundMusic();

        // // Get winner
        // string winner = PlayerPrefs.GetString("RoundWinner", "");

        // // Play corresponding audio clip
        // if (winner == "Player One")
        // {
        //     Play(P1Win);
        // }
        // else if (winner == "Player Two")
        // {
        //     Play(P2Win);
        // }
        // else → play nothing
    }
}
