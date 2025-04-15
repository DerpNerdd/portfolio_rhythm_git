using System.Collections;
using UnityEngine;

public class SettingsBarWithContentController : MonoBehaviour
{
    [Header("Settings Bar (3D)")]
    [Tooltip("The 3D settings bar that slides in and out.")]
    public Transform settingsBar;
    [Tooltip("World space position when the settings bar is open.")]
    public Vector3 openPosition = new Vector3(0f, 0f, 0f);
    [Tooltip("World space position when the settings bar is closed.")]
    public Vector3 closedPosition = new Vector3(-10f, 0f, 0f);
    [Tooltip("Slide animation duration (in seconds).")]
    public float slideDuration = 0.5f;
    
    [Header("Settings Content (Canvas)")]
    [Tooltip("The RectTransform of the SettingsContent canvas.")]
    public RectTransform settingsContent;
    [Tooltip("Anchored position when the settings content is fully visible (open).")]
    public Vector2 contentOpenAnchoredPos = new Vector2(0f, 0f);
    [Tooltip("Anchored position when the settings content is hidden (closed).")]
    public Vector2 contentClosedAnchoredPos = new Vector2(0f, 0f);
    
    [Header("Dim Overlay (Optional)")]
    [Tooltip("Reference to the DimOverlayController that handles background dimming.")]
    public DimOverlayController dimOverlay;
    
    private bool isOpen = false;
    private bool isSliding = false;

    // Call this method from your settings button's OnClick event.
    public void ToggleSettingsBar()
    {
        if (isSliding) return;
        if (isOpen)
            SlideOut();
        else
            SlideIn();
    }

    public void SlideIn()
    {
        if (isSliding) return;
        if (dimOverlay != null)
            dimOverlay.FadeIn();
        StartCoroutine(SlideRoutine(closedPosition, openPosition, contentClosedAnchoredPos, contentOpenAnchoredPos, slideDuration));
        isOpen = true;
    }

    public void SlideOut()
    {
        if (isSliding) return;
        if (dimOverlay != null)
            dimOverlay.FadeOut();
        StartCoroutine(SlideRoutine(openPosition, closedPosition, contentOpenAnchoredPos, contentClosedAnchoredPos, slideDuration));
        isOpen = false;
    }

    private IEnumerator SlideRoutine(Vector3 fromPos, Vector3 toPos, Vector2 fromContentPos, Vector2 toContentPos, float duration)
    {
        isSliding = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // Slide the 3D settings bar in world space.
            settingsBar.position = Vector3.Lerp(fromPos, toPos, t);
            // Also update the anchored position of the settings content.
            if (settingsContent != null)
            {
                settingsContent.anchoredPosition = Vector2.Lerp(fromContentPos, toContentPos, t);
            }
            yield return null;
        }
        settingsBar.position = toPos;
        if (settingsContent != null)
        {
            settingsContent.anchoredPosition = toContentPos;
        }
        isSliding = false;
    }
}
