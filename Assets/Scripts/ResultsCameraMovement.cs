using UnityEngine;

public class ResultsCameraMovement : MonoBehaviour
{
    public Transform target; 
    public float rotationSpeed = 50f; 
    public Vector3 rotationAxis = Vector3.up; 

    void LateUpdate()
    {
        if (target != null)
            transform.RotateAround(target.position, rotationAxis, rotationSpeed * Time.deltaTime);
        else
            Debug.Log("Target is null");
    }
}
