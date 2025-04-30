using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to a spherical bubble GameObject to handle hover-pulse and click-to-load behavior.
/// Requires a PhysicsRaycaster on the camera and an EventSystem in the scene.
/// </summary>
public class BubbleSphereButton  : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Tooltip("Name of the scene to load when this bubble is clicked.")]
    public string sceneToLoad;

    private Vector3 originalScale;
    private Coroutine pulseRoutine;

    void Awake()
    {
        // Cache the starting scale to pulse relative to
        originalScale = transform.localScale;
    }

    /// <summary>
    /// Triggered when the pointer hovers over the bubble.
    /// Starts a continuous pulsing coroutine.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Begin pulsing animation
        pulseRoutine = StartCoroutine(PulseEffect());
    }

    /// <summary>
    /// Triggered when the pointer exits the bubble.
    /// Stops pulsing and resets scale.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);
        transform.localScale = originalScale;
    }

    /// <summary>
    /// Triggered on click. Loads the assigned scene, if any.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
    }

    /// <summary>
    /// Coroutine that continuously scales the bubble up/down to create a "jiggle" effect.
    /// </summary>
    private IEnumerator PulseEffect()
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * 4f; // speed multiplier
            float scaleFactor = 1f + Mathf.Sin(t) * 0.1f; // pulse amplitude
            transform.localScale = originalScale * scaleFactor;
            yield return null;
        }
    }
}
