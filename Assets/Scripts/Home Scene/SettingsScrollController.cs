using UnityEngine;

public class SettingsScroll3DController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The scrolling content (e.g., SettingsText) that is taller than the viewport. Its local position will be adjusted to scroll.")]
    public Transform content;
    [Tooltip("The viewport (visible area) that displays the content. (For dimension purposes, set this to your settingsBar.)")]
    public Transform viewport;
    [Tooltip("The scrollbar background (BG), a 3D quad whose center determines the track's vertical bounds.")]
    public Transform scrollbarBG;

    [Header("Dimensions (in World Units)")]
    [Tooltip("The height of the scrollbar track (BG). Set to 0.9459964.")]
    public float trackHeight = 0.9459964f;
    [Tooltip("The total height of the content (SettingsText). Set to 11.83535.")]
    public float contentHeight = 11.83535f;
    [Tooltip("The visible (viewport) height. Set to 8.357608.")]
    public float viewportHeight = 8.357608f;

    [Header("Scroll Adjustment")]
    [Tooltip("Reduce the maximum scroll offset by this amount (world units) to manually prevent over-scrolling.")]
    public float clampAdjustment = 3.05f;
    [Tooltip("Manually adjust the height of the pink Bar (thumb). Set a value greater than 0 to override the computed height.")]
    public float manualThumbHeight = 0f;

    // Private fields:
    private float thumbHeight;      // Height of the thumb (Bar)
    private float allowedRange;     // Allowed vertical movement range for the thumb's center
    private float topEdge;          // Top edge of the scrollbar track (from BG)
    private float bottomEdge;       // Bottom edge of the scrollbar track (from BG)
    
    // Dragging calculations:
    private bool dragging = false;
    private float pointerOffsetY = 0f;
    // Store the initial local position of the content (so only its Y changes when scrolling).
    private Vector3 initialContentLocalPos;
    // Total scrollable offset (contentHeight - viewportHeight).
    private float maxDelta;

    void Start()
    {
        if (content == null || viewport == null || scrollbarBG == null)
        {
            Debug.LogError("SettingsScroll3DController: Please assign content, viewport, and scrollbarBG in the Inspector.");
            return;
        }

        // Calculate top and bottom edges of the track using the BG's center.
        topEdge = scrollbarBG.position.y + trackHeight / 2f;
        bottomEdge = scrollbarBG.position.y - trackHeight / 2f;

        // Compute the thumb height based on the ratio (viewportHeight / contentHeight) * trackHeight.
        // If manualThumbHeight is set (> 0), use that instead.
        float computedThumbHeight = (viewportHeight / contentHeight) * trackHeight;
        thumbHeight = (manualThumbHeight > 0f) ? manualThumbHeight : computedThumbHeight;
        thumbHeight = Mathf.Min(thumbHeight, trackHeight);

        // Allowed range is from (bottomEdge + thumbHeight/2) to (topEdge - thumbHeight/2).
        allowedRange = (topEdge - thumbHeight / 2f) - (bottomEdge + thumbHeight / 2f);

        // Adjust the thumb's local scale so its height becomes thumbHeight.
        Vector3 newScale = transform.localScale;
        newScale.y = thumbHeight;
        transform.localScale = newScale;

        // Position the thumb so that its center is flush with the top of the track.
        Vector3 pos = transform.position;
        pos.y = topEdge - (thumbHeight / 2f);
        transform.position = pos;

        // Save the initial local position of the content.
        initialContentLocalPos = content.localPosition;
        // Compute the total scrollable offset.
        maxDelta = contentHeight - viewportHeight;
    }

    void OnMouseDown()
    {
        dragging = true;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Mathf.Abs(Camera.main.transform.position.z - transform.position.z)
        ));
        pointerOffsetY = mouseWorld.y - transform.position.y;
    }

    void OnMouseDrag()
    {
        if (!dragging)
            return;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Mathf.Abs(Camera.main.transform.position.z - transform.position.z)
        ));
        float newY = mouseWorld.y - pointerOffsetY;
        // Clamp newY so that the thumb's center remains between the lower and upper clamps.
        float lowerClamp = bottomEdge + thumbHeight / 2f;
        float upperClamp = topEdge - thumbHeight / 2f;
        newY = Mathf.Clamp(newY, lowerClamp, upperClamp);
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;

        UpdateContentPosition();
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    void UpdateContentPosition()
    {
        // Calculate normalized scroll value (0 at top; 1 at bottom) based on the thumb's center.
        float currentCenter = transform.position.y;
        float normalized = 1f - ((currentCenter - (bottomEdge + thumbHeight / 2f)) / allowedRange);
        
        // Compute the effective maximum offset for the content.
        float effectiveMaxDelta = Mathf.Max(maxDelta - clampAdjustment, 0f);
        float contentOffset = normalized * effectiveMaxDelta;
        
        Vector3 newLocalPos = initialContentLocalPos;
        newLocalPos.y = initialContentLocalPos.y + contentOffset;
        content.localPosition = newLocalPos;
    }
}
