using UnityEngine;

public class Fencer : MonoBehaviour
{
    private int fencerId;
    private Camera cam;

    public void Initialize(int fn)
    {
        // set id
        fencerId = fn;

        // set camera position
        cam = GetComponentInChildren<Camera>(); 
        Rect r = cam.rect;
        r.x = (fencerId == 1 ? 0f : 0.5f);
        cam.rect = r;
    }
}
