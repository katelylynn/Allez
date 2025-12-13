/*
    Background Music Player
    Plays the background music in the results scene.
*/

using UnityEngine;
using TMPro;

public class BackgroundMusicPlayer : MonoBehaviour
{
    public AudioClip backgroundMusic;
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            source.clip = backgroundMusic;
            source.loop = true;
            source.volume = 1f;
            source.Play();
        }
    }
}
