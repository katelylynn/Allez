using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RoundStartCountDown : MonoBehaviour
{
    [Header("Countdown")]
    public float countdownTickDuration = 1.5f;
    public Transform textParent;
    public AudioSource countdownAudio;

    [Header("UI")]
    public TMP_Text roundWinner;

    [Header("SFX")]
    [SerializeField] private AudioClip ouch1Clip; // P1
    [SerializeField] private AudioClip ouch2Clip; // P2

    private TMP_Text[] countdownTexts;
    private float countdownTime = 3f;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // Validate early so you see clear messages in Console
        if (!audioSource) Debug.LogError("[RoundStartCountDown] Missing AudioSource on this GameObject.");
        if (!countdownAudio) Debug.LogWarning("[RoundStartCountDown] 'countdownAudio' not assigned.");
        if (!roundWinner) Debug.LogWarning("[RoundStartCountDown] 'roundWinner' TMP_Text not assigned.");
        if (!textParent) Debug.LogWarning("[RoundStartCountDown] 'textParent' not assigned.");
    }

    public IEnumerator Run()
    {
        countdownTexts = textParent ? textParent.GetComponentsInChildren<TMP_Text>(true) : new TMP_Text[0];
        yield return null;
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(Countdown());
    }

    public IEnumerator Countdown()
    {
        float count = countdownTime;
        while (count > 0f)
        {
            int i = Mathf.RoundToInt(countdownTime - count);

            foreach (var t in countdownTexts)
                t.gameObject.SetActive(false);

            if (i >= 0 && i < countdownTexts.Length)
                countdownTexts[i].gameObject.SetActive(true);

            if (countdownAudio != null && !countdownAudio.isPlaying)
                countdownAudio.Play();

            yield return new WaitForSeconds(countdownTickDuration);
            count--;
        }

        foreach (var t in countdownTexts)
            t.gameObject.SetActive(false);
    }

    public void DisplayWinner(int winner)
    {
        // pick clip based on winner
        AudioClip clip = (winner == 0) ? ouch1Clip : ouch2Clip;

        if (clip == null)
        {
            Debug.LogWarning($"[RoundStartCountDown] Winner clip is NULL (winner={winner}). Assign ouch1Clip/ouch2Clip in Inspector.");
        }
        else if (audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }

        if (roundWinner != null)
        {
            roundWinner.color = (winner == 0) ? Color.blue : Color.red;
            roundWinner.text = (winner == 0 ? "Player one" : "Player two") + " scores a touch!";
            roundWinner.gameObject.SetActive(true);
        }
    }

    public void HideWinner() => roundWinner?.gameObject.SetActive(false);
}

// using System.Collections;
// using TMPro;
// using Unity.VisualScripting;
// using UnityEngine;

// public class RoundStartCountDown : MonoBehaviour
// {
//     public float countdownTickDuration = 1.5f;
//     public Transform textParent;
//     private TMP_Text[] countdownTexts;
//     private float countdownTime = 3f;
//     public TMP_Text roundWinner;
    
//     // Audio sources : don't need audio source with extra object with AudioClip : needs to be cleaend up later
//     public AudioSource countdownAudio;
//     public AudioClip ouch1Clip;
//     public AudioClip ouch2Clip;
//     private AudioSource audioSource;

//     void Start() {
//         audioSource = GetComponent<AudioSource>();
//     }

//     public IEnumerator Run()
//     {
//         countdownTexts = textParent.GetComponentsInChildren<TMP_Text>(true);

//         //Wait a short time for loading to sync up audio
//         yield return null;
//         yield return new WaitForEndOfFrame();
//         yield return StartCoroutine(Countdown());
//     }

//     public IEnumerator Countdown()
//     {
        
//         float count = countdownTime;
//         while (count > 0)
//         {
            
//             int i = Mathf.RoundToInt(countdownTime - count);

//             foreach (var t in countdownTexts)
//                 t.gameObject.SetActive(false);

//             if (i >= 0 && i < countdownTexts.Length)
//                 countdownTexts[i].gameObject.SetActive(true);
//             if (!countdownAudio.isPlaying)
//             {
//                 countdownAudio.Play();
//             }
//             yield return new WaitForSeconds(countdownTickDuration);
//             count--;
//         }

//         foreach (var t in countdownTexts)
//             t.gameObject.SetActive(false);
//     }

//     public void DisplayWinner(int winner)
//     {
//         string winnerText;

//         if (winner == 0)
//         {
//             audioSource.PlayOneShot(ouch1Clip);
//             winnerText = "Player one";
//             roundWinner.color = Color.blue;
//         }
//         else
//         {
//             audioSource.PlayOneShot(ouch2Clip);
//             winnerText = "Player two";
//             roundWinner.color = Color.red;
//         }
//         winnerText += " scores a touch!";

//         roundWinner.text = winnerText;  
//         roundWinner.gameObject.SetActive(true);
//     }

//     public void HideWinner()
//     {
//         roundWinner.gameObject.SetActive(false);
//     }
// }
