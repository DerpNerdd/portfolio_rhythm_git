using System.Collections;
using UnityEngine;

public class SettingsBar3DController : MonoBehaviour
{
    [Header("Settings Bar Reference")]
    [Tooltip("Reference to the 3D settings bar that will be animated. If left empty, it uses this GameObject's transform.")]
    public Transform settingsBar;

    [Header("Slide Positions (World Space)")]
    [Tooltip("The world-space position where the settings bar is fully visible (open).")]
    public Vector3 openPosition = new Vector3(0f, 0f, 0f);
    [Tooltip("The world-space position where the settings bar is off-screen to the left (closed).")]
    public Vector3 closedPosition = new Vector3(-10f, 0f, 0f);

    [Header("Animation Settings")]
    [Tooltip("Time (in seconds) for the slide animation.")]
    public float slideDuration = 0.5f;

    [Header("Dim Overlay")]
    [Tooltip("Reference to the DimOverlayController that handles dimming the background.")]
    public DimOverlayController dimOverlay;

    private bool isOpen = false;
    private bool isSliding = false;

    void Start()
    {
        if (settingsBar == null)
        {
            settingsBar = transform;
        }
        // Start in the closed state.
        settingsBar.position = closedPosition;
    }

    // This public method can be linked to your settings button's OnClick event.
    public void ToggleSettingsBar()
    {
        if (isSliding)
            return;

        if (isOpen)
            SlideOut();
        else
            SlideIn();
    }

    public void SlideIn()
    {
        if (isSliding)
            return;
        
        // Fade in the dim overlay if it's assigned.
        if (dimOverlay != null)
        {
            dimOverlay.FadeIn();
        }
        
        StartCoroutine(SlideRoutine(settingsBar.position, openPosition, slideDuration));
        isOpen = true;
    }

    public void SlideOut()
    {
        if (isSliding)
            return;
        
        // Fade out the dim overlay if it's assigned.
        if (dimOverlay != null)
        {
            dimOverlay.FadeOut();
        }
        
        StartCoroutine(SlideRoutine(settingsBar.position, closedPosition, slideDuration));
        isOpen = false;
    }

    private IEnumerator SlideRoutine(Vector3 from, Vector3 to, float duration)
    {
        isSliding = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            settingsBar.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
        settingsBar.position = to;
        isSliding = false;
    }

    void Update()
    {
        // When the bar is open, detect clicks that are not within the settings bar.
        if (isOpen && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool clickedOnBar = false;

            // Fire a raycast and check if the hit belongs to the settings bar or any of its children.
            if (Physics.Raycast(ray, out hit))
            {
                // If the hit collider's transform is either the settingsBar itself or is a child of it, consider it a click on the bar.
                if (hit.collider.transform == settingsBar || hit.collider.transform.IsChildOf(settingsBar))
                {
                    clickedOnBar = true;
                }
            }

            // Only slide out if the click did NOT hit the settings bar or any of its children.
            if (!clickedOnBar)
            {
                SlideOut();
            }
        }
    }
}
