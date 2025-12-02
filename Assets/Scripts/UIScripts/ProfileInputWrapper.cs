using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ProfileInputWrapper : MonoBehaviour
{
    [Header("The TMP input to edit when you press A")]
    [SerializeField] private TMP_InputField inputField;

    [Header("Overlay to show when the wrapper is focused")]
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

        // Overlay ON only when wrapper is selected
        if (overlay != null)
            overlay.SetActive(wrapperSelected);

        // --- A on wrapper -> enter input field ---
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

        // --- While editing the input field ---
        if (inputSelected)
        {
            // B -> exit input and go back to wrapper
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

        // Anything else selected -> overlay off
        if (overlay != null)
            overlay.SetActive(false);
    }

    public void SetInputField(TMP_InputField field)
    {
        inputField = field;
    }
}
