using System.Collections;
using TMPro;
using UnityEngine;

public class RoundStartCountDown : MonoBehaviour
{

    public float countdownTickDuration = 1.5f;
    public Transform textParent;
    private TMP_Text[] countdownTexts;
    private float countdownTime = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public IEnumerator Run()
    {
        //if (textParent == null)
        //{
        //    Debug.LogError("Text parent not assigned!");
        //    yield return;
        //}
        countdownTexts = textParent.GetComponentsInChildren<TMP_Text>(true);
        //Debug.Log("Arr length: " + countdownTexts.Length);

        yield return StartCoroutine(Countdown());
    }

    public IEnumerator Countdown()
    {
        float count = countdownTime;
        
        while (count > 0)
        {
            //Debug.Log("looping# " + count);
            int i = Mathf.RoundToInt(countdownTime - count);

            foreach (var t in countdownTexts)
                t.gameObject.SetActive(false);


            if (i >= 0 && i < countdownTexts.Length)
            {
                Debug.Log("Found text " + countdownTexts[i].text);
                countdownTexts[i].gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(countdownTickDuration);
            count--;
        }
        foreach (var t in countdownTexts)
            t.gameObject.SetActive(false);

    }
  
}
