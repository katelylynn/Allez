using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class TimerModeBGMController : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Sound Clips")]
    public AudioClip TimerBGM;

    private AudioSource source;
    private float timer = 0f;

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.clip = TimerBGM;

        source.Play();
    }

    void Update()
    {
        if (gameManager.pointsToWin != 0) {
            return;
        }

        // Sometimes gameManaer.elapsedTime doesn't work so randomly. 
        // So i'm using timer separately.
        timer += Time.deltaTime;

        if (timer <= 30f)
        {
            source.pitch = 1.0f;   
        }
        else if (timer <= 60f)
        {
            source.pitch = 1.05f;   
        }
        else if (timer <= 90f)
        {
            source.pitch = 1.1f;   
        }
        else
        {
            source.pitch = 1.15f;  
        }
    }
}
