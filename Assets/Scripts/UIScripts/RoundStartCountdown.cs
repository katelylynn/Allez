using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RoundStartCountDown : MonoBehaviour
{
    public float countdownTickDuration = 1.5f;
    public Transform textParent;
    private TMP_Text[] countdownTexts;
    private float countdownTime = 3f;
    public TMP_Text roundWinner;
    
    // Audio sources : don't need audio source with extra object with AudioClip : needs to be cleaend up later
    public AudioSource countdownAudio;
    public AudioClip ouch1Clip;
    public AudioClip ouch2Clip;
    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public IEnumerator Run()
    {
        countdownTexts = textParent.GetComponentsInChildren<TMP_Text>(true);

        //Wait a short time for loading to sync up audio
        yield return null;
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(Countdown());
    }

    public IEnumerator Countdown()
    {
        
        float count = countdownTime;
        while (count > 0)
        {
            
            int i = Mathf.RoundToInt(countdownTime - count);

            foreach (var t in countdownTexts)
                t.gameObject.SetActive(false);

            if (i >= 0 && i < countdownTexts.Length)
                countdownTexts[i].gameObject.SetActive(true);
            if (!countdownAudio.isPlaying)
            {
                countdownAudio.Play();
            }
            yield return new WaitForSeconds(countdownTickDuration);
            count--;
        }

        foreach (var t in countdownTexts)
            t.gameObject.SetActive(false);
    }

    public void DisplayWinner(int winner)
    {
        string winnerText;

        if (winner == 0)
        {
            audioSource.PlayOneShot(ouch1Clip);
            winnerText = "Player one";
            roundWinner.color = Color.blue;
        }
        else
        {
            audioSource.PlayOneShot(ouch2Clip);
            winnerText = "Player two";
            roundWinner.color = Color.red;
        }
        winnerText += " scores a touch!";

        roundWinner.text = winnerText;  
        roundWinner.gameObject.SetActive(true);
    }

    public void HideWinner()
    {
        roundWinner.gameObject.SetActive(false);
    }
}
