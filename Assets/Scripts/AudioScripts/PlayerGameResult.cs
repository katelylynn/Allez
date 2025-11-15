using UnityEngine;
using TMPro;

public class PlayerGameResult : MonoBehaviour
{
    TMP_Text[] resultsText;


    [Header("Sound Clips")]
    public AudioClip P1Win;
    public AudioClip P2Win;
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
        if (clip != null) source.PlayOneShot(clip, volume);
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
        resultsText = gameObject.GetComponentsInChildren<TMP_Text>();

        // start looping background music first
        PlayBackgroundMusic();

        foreach (TMP_Text text in resultsText)
        {
            if (text.name == "WinText")
            {
                string winner = PlayerPrefs.GetString("RoundWinner");
                int rounds = PlayerPrefs.GetInt("CurrentRound");

                //text.text = $"{winner} wins in {rounds} rounds!";

                if (winner.Equals("Player One"))
                    Play(P1Win);
                else
                    Play(P2Win);
            }
        }
    }
}
    

