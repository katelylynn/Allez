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



        if (roundWinner != null)
        {
            roundWinner.color = (winner == 0) ? Color.blue : Color.red;
            roundWinner.text = (winner == 0 ? "Player one" : "Player two") + " scores a touch!";
            roundWinner.gameObject.SetActive(true);
        }
    }

    public void HideWinner() => roundWinner?.gameObject.SetActive(false);
}
