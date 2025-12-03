using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProfileSelection : MonoBehaviour
{
    [Header("First item inside the scroll view")]
    [SerializeField] private Selectable firstItem;

    [Header("Overlay to show when scroll view is focused")]
    public GameObject overlay;

    private EventSystem es;
    private Selectable scrollSelectable;

    private ScrollRect scrollRect;
    private RectTransform viewport;
    private RectTransform content;

    private readonly Vector3[] itemCorners = new Vector3[4];
    private readonly Vector3[] viewCorners = new Vector3[4];

    private void Awake()
    {
        es = EventSystem.current;
        scrollSelectable = GetComponent<Selectable>();

        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            viewport = scrollRect.viewport;
            content  = scrollRect.content;
        }

        // Ensure overlay starts off
        if (overlay != null)
            overlay.SetActive(false);
    }

    private void Update()
    {
        if (es == null) return;

        GameObject current = es.currentSelectedGameObject;
        if (current == null) return;

        bool isScrollSelected = (current == gameObject);

        // --- Toggle overlay state ---
        if (overlay != null)
            overlay.SetActive(isScrollSelected);

        // --- A on Scroll View row -> enter list ---
        if (isScrollSelected)
        {
            if (Input.GetButtonDown("Submit") && firstItem != null)
            {
                firstItem.Select();
            }
            return;
        }

        // --- If child item is selected ---
        if (IsChildOfContent(current.transform))
        {
            // B -> return to Scroll View + turn overlay on
            if (Input.GetButtonDown("Cancel"))
            {
                if (scrollSelectable != null)
                    scrollSelectable.Select();
                else
                    es.SetSelectedGameObject(gameObject);

                if (overlay != null)
                    overlay.SetActive(true);

                return;
            }

            // Make sure selected child is visible
            RectTransform itemRT = current.GetComponent<RectTransform>();
            EnsureVisible(itemRT);

            // Overlay should be OFF while navigating inside the list
            if (overlay != null)
                overlay.SetActive(false);

            return;
        }

        // --- Any other UI selection -> disable overlay ---
        if (overlay != null)
            overlay.SetActive(false);
    }

    private bool IsChildOfContent(Transform t)
    {
        return content != null && t != null && t.IsChildOf(content);
    }

    public void SetFirstItem(Selectable item)
    {
        firstItem = item;
    }

    private void EnsureVisible(RectTransform item)
    {
        if (item == null || viewport == null || content == null) return;

        item.GetWorldCorners(itemCorners);
        viewport.GetWorldCorners(viewCorners);

        float delta = 0f;

        float itemBottom = itemCorners[0].y;
        float itemTop    = itemCorners[1].y;
        float viewBottom = viewCorners[0].y;
        float viewTop    = viewCorners[1].y;

        if (itemBottom < viewBottom)
        {
            delta = viewBottom - itemBottom;
        }
        else if (itemTop > viewTop)
        {
            delta = viewTop - itemTop;
        }

        if (Mathf.Abs(delta) > 0.01f)
        {
            content.position += new Vector3(0f, delta, 0f);
        }
    }
}
