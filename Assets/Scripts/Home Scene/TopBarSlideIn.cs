using System.Collections;
using UnityEngine;

public class TopBarSlideIn3D : MonoBehaviour
{
    [Tooltip("Duration of the slide in animation (seconds).")]
    public float slideDuration = 1f;
    [Tooltip("Target world position for the top navbar when slid into place.")]
    public Vector3 targetPosition;
    
    private Vector3 initialPosition;

    void Start()
    {
        // Record the initial position from which the nav bar will slide in.
        initialPosition = transform.position;
        Debug.Log("TopBarSlideIn3D: Initial position recorded as " + initialPosition);
    }

    public void SlideIn()
    {
        Debug.Log("TopBarSlideIn3D: SlideIn() called. Target position is " + targetPosition);
        StartCoroutine(SlideInCoroutine());
    }

    IEnumerator SlideInCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float easeT = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(initialPosition, targetPosition, easeT);
            yield return null;
        }
        transform.position = targetPosition;
        Debug.Log("TopBarSlideIn3D: Slide complete. Final position is " + transform.position);
    }
}
