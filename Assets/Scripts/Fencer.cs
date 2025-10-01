using UnityEngine;

public class Fencer : MonoBehaviour
{
    // static fields
    private const int F0 = 0;
    private const int F1 = 1;

    // instance fields
    private int fencerId;
    private Camera cam;

    // starting position
    private Vector3[] startingPos = {
        new Vector3(0, 0, -5),
        new Vector3(0, 0, 5)
    };
    private Quaternion[] startingRot = {
        Quaternion.Euler(0f, 0f, 0f),
        Quaternion.Euler(0f, 180f, 0f)
    };

    // called by scene initializer
    public void Initialize(int fn)
    {
        // set id
        fencerId = fn;

        // set camera position
        cam = GetComponentInChildren<Camera>(); 
        Rect r = cam.rect;
        r.x = (fencerId == F0 ? 0f : 0.5f);
        cam.rect = r;

        // set fencer position, deactivating to overcome rigidbody
        gameObject.SetActive(false);
        gameObject.transform.position = startingPos[fencerId];
        gameObject.transform.rotation = startingRot[fencerId];
        gameObject.SetActive(true);
    }
}
