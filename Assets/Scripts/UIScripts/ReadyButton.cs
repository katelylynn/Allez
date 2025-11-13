using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    public bool isReady = false;

    public void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void Update()
    {
        if (isReady)
        {
            gameObject.GetComponent<Image>().color = Color.green;
        }
        else
        {
            gameObject.GetComponent<Image>().color = Color.yellow;
        }
    }
    public void OnClick()
    {
        //Debug.Log("clicked");
        isReady = !isReady;
    }
}
