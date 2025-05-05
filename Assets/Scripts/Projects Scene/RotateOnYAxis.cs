using UnityEngine;

public class RotateOnYAxis : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second.")]
    public float rotationSpeed = 10f;

    void Update()
    {
        // Rotate only around Y, in local space
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }
}
