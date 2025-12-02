using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MyButton : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
    public Action<string, int> callback;
    public TMP_Text text;
    public int playerId;

    // Mouse click
    public void OnPointerClick(PointerEventData eventData)
    {
        callback?.Invoke(text.text, playerId);
    }

    // Controller "A" / Keyboard Enter
    public void OnSubmit(BaseEventData eventData)
    {
        callback?.Invoke(text.text, playerId);
    }

    public void SetText(string s)
    {
        text.SetText(s);
    }
}
