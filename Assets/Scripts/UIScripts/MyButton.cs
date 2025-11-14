using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MyButton : MonoBehaviour, IPointerClickHandler
{
    public Action<string, int> callback;
    public TMP_Text text;
    public int playerId;
    public void OnPointerClick(PointerEventData eventData)
    {
        callback?.Invoke(text.text, playerId);
    }

    public void SetText(string s)
    {
        text.SetText(s);
    }
}
