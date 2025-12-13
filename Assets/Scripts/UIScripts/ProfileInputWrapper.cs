using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ProfileInputWrapper : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject overlay;

    private EventSystem es;
    private Selectable wrapperSelectable;

    private void Awake()
    {
        es = EventSystem.current;
        wrapperSelectable = GetComponent<Selectable>();

        if (overlay != null)
            overlay.SetActive(false);
    }

    private void Update()
    {
        if (es == null) return;

        GameObject current = es.currentSelectedGameObject;
        if (current == null) return;

        bool wrapperSelected = (current == gameObject);
        bool inputSelected   = (inputField != null && current == inputField.gameObject);

        if (overlay != null)
            overlay.SetActive(wrapperSelected);

        if (wrapperSelected)
        {
            if (Input.GetButtonDown("Submit") && inputField != null)
            {
                inputField.Select();
                inputField.ActivateInputField();

                if (overlay != null)
                    overlay.SetActive(false);
            }
            return;
        }

        if (inputSelected)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                inputField.DeactivateInputField();

                if (wrapperSelectable != null)
                    wrapperSelectable.Select();
                else
                    es.SetSelectedGameObject(gameObject);

                if (overlay != null)
                    overlay.SetActive(true);
            }

            return;
        }

        if (overlay != null)
            overlay.SetActive(false);
    }

    public void SetInputField(TMP_InputField field)
    {
        inputField = field;
    }
}
