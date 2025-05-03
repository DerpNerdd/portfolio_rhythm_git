using UnityEngine;
using System.Collections;  

[RequireComponent(typeof(Collider))]
public class ModelRotator : MonoBehaviour
{
    [Tooltip("How fast the model spins in response to drag.")]
    public float rotationSpeed = 200f;
    [Tooltip("How long (in seconds) to tween back to the original rotation.")]
    public float returnDuration = 0.5f;

    bool    dragging = false;
    Vector3 lastPointerPos;
    Quaternion originalRotation;
    Coroutine returnRoutine;

    void OnMouseDown()
    {
        // Capture the rotation at the start of the drag
        originalRotation = transform.rotation;
        // Stop any in-flight return tween
        if (returnRoutine != null) StopCoroutine(returnRoutine);

        dragging = true;
        lastPointerPos = Input.mousePosition;
    }

    void OnMouseUp()
    {
        dragging = false;
        // Kick off the return tween
        returnRoutine = StartCoroutine(ReturnToOriginal());
    }

    void Update()
    {
        if (!dragging)
            return;

        Vector3 currentPos = Input.mousePosition;
        Vector3 delta      = currentPos - lastPointerPos;

        float yaw   = -delta.x * rotationSpeed * Time.deltaTime;
        float pitch =  delta.y * rotationSpeed * Time.deltaTime;

        // Rotate around world up and camera right
        transform.Rotate(Vector3.up, yaw, Space.World);
        transform.Rotate(Camera.main.transform.right, pitch, Space.World);

        lastPointerPos = currentPos;
    }

    IEnumerator ReturnToOriginal()
    {
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / returnDuration);
            transform.rotation = Quaternion.Slerp(startRot, originalRotation, t);
            yield return null;
        }

        transform.rotation = originalRotation;
        returnRoutine = null;
    }
}
