using System.Collections;
using UnityEngine;

public class SettingsBar3DController : MonoBehaviour
{
    [Header("Settings Bar Reference")]
    [Tooltip("Reference to the 3D settings bar that will be animated. If left empty, it uses this GameObject's transform.")]
    public Transform settingsBar;
    
    [Tooltip("The root object that contains the settings bar and all its children that should be considered 'inside'. If not set, settingsBar is used.")]
    public Transform settingsAreaRoot;
    
    [Header("Additional Ignore Area")]
    [Tooltip("Clicks on this object (or any of its children) will be ignored (won't close the settings bar). For example, assign the PinkCircleQuad here.")]
    public Transform ignoreClickArea;
    
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
    
    [Header("Main Buttons Disable (Optional)")]
    [Tooltip("The CanvasGroup of the main buttons you want to disable when the settings bar is open.")]
    public CanvasGroup mainButtonsGroup;

    private bool isOpen = false;
    private bool isSliding = false;

    void Start()
    {
        if (settingsBar == null)
        {
            settingsBar = transform;
        }
        // If settingsAreaRoot isn't set, default to settingsBar.
        if (settingsAreaRoot == null)
        {
            settingsAreaRoot = settingsBar;
        }
        // Start in the closed state.
        settingsBar.position = closedPosition;
    }

    // This public method should be linked to your settings button's OnClick event.
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

        if (dimOverlay != null)
            dimOverlay.FadeIn();

        if (mainButtonsGroup != null)
        {
            mainButtonsGroup.interactable = false;
            mainButtonsGroup.blocksRaycasts = false;
        }
        StartCoroutine(SlideRoutine(settingsBar.position, openPosition, slideDuration));
        isOpen = true;
    }

    public void SlideOut()
    {
        if (isSliding)
            return;

        if (dimOverlay != null)
            dimOverlay.FadeOut();

        if (mainButtonsGroup != null)
        {
            mainButtonsGroup.interactable = true;
            mainButtonsGroup.blocksRaycasts = true;
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
        // Only check for outside clicks if the settings bar is open.
        if (isOpen && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool clickedInside = false;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit collider belongs to settingsAreaRoot (or its children).
                if (hit.collider != null)
                {
                    if (hit.collider.transform == settingsAreaRoot || hit.collider.transform.IsChildOf(settingsAreaRoot))
                    {
                        clickedInside = true;
                    }
                    // Also, if the hit collider belongs to ignoreClickArea (or its children), consider it inside.
                    else if (ignoreClickArea != null && (hit.collider.transform == ignoreClickArea || hit.collider.transform.IsChildOf(ignoreClickArea)))
                    {
                        clickedInside = true;
                    }
                }
            }

            // If the click did NOT hit any object in the settings area or ignore area, close the bar.
            if (!clickedInside)
            {
                SlideOut();
            }
        }
    }
}
