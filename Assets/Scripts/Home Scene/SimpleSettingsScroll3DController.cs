using UnityEngine;

public class SimpleSettingsScroll3DController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The scrolling content (e.g., SettingsText) that will be moved vertically. Its localPosition is adjusted.")]
    public Transform content;
    [Tooltip("The viewport (visible area) that displays the content. This is your settingsBar; its position is used for reference.")]
    public Transform viewport;
    [Tooltip("The scrollbar background (BG). Its position and scale determine the scroll track's top and bottom edges.")]
    public Transform scrollbarBG;

    [Header("Content & Viewport Dimensions (World Units)")]
    [Tooltip("The total height of the content (e.g., SettingsText). Set to 11.83535.")]
    public float contentHeight = 11.83535f;
    [Tooltip("The visible height of the content (viewport). Set to 8.357608.")]
    public float viewportHeight = 8.357608f;

    [Header("Scroll Adjustment")]
    [Tooltip("Manually adjust the final content offset (world units) to fine-tune where scrolling stops.")]
    public float manualStopOffset = 0f;

    // Private fields:
    private Transform thumb;                  // This object (the pink Bar)
    private float thumbHeight;                // Computed thumb height (in world units)
    private float trackTop;                   // Top edge of the scroll track (BG)
    private float trackBottom;                // Bottom edge of the scroll track (BG)
    private float trackRange;                 // (trackTop - trackBottom)
    private Vector3 initialContentLocalPos;   // The original local position of content (used as base offset)
    private float scrollableContentRange;     // contentHeight - viewportHeight

    // Dragging variables
    private bool dragging = false;
    private float pointerOffsetY = 0f;

    void Start()
    {
        if (content == null || viewport == null || scrollbarBG == null)
        {
            Debug.LogError("SimpleSettingsScroll3DController: Please assign content, viewport, and scrollbarBG in the Inspector.");
            return;
        }
        
        // Cache reference to this object's transform as the thumb.
        thumb = transform;
        
        // Get the track's top and bottom from BG.
        // We'll assume the BG quad's height in world units equals its lossyScale.y.
        trackRange = scrollbarBG.lossyScale.y;
        trackTop = scrollbarBG.position.y + trackRange / 2f;
        trackBottom = scrollbarBG.position.y - trackRange / 2f;
        
        // Compute thumb height as the proportion of visible content to total content scaled to the track.
        // thumbHeight = (viewportHeight / contentHeight) * trackRange
        thumbHeight = (viewportHeight / contentHeight) * trackRange;
        
        // Adjust the thumb's scale so its y-scale becomes thumbHeight.
        Vector3 ts = thumb.localScale;
        ts.y = thumbHeight;
        thumb.localScale = ts;
        
        // Position the thumb so that its center is flush with the top of the BG.
        Vector3 tp = thumb.position;
        tp.y = trackTop - (thumbHeight / 2f);
        thumb.position = tp;
        
        // Save the initial local position of the content.
        initialContentLocalPos = content.localPosition;
        scrollableContentRange = contentHeight - viewportHeight;
    }

    void OnMouseDown()
    {
        dragging = true;
        Vector3 mouseWorld = GetMouseWorldPosition();
        pointerOffsetY = mouseWorld.y - thumb.position.y;
    }

    void OnMouseDrag()
    {
        if (!dragging) return;
        Vector3 mouseWorld = GetMouseWorldPosition();
        float newY = mouseWorld.y - pointerOffsetY;
        // Clamp the newY so that the thumb's center never leaves the BG's boundaries.
        float lowerClamp = trackBottom + (thumbHeight / 2f);
        float upperClamp = trackTop - (thumbHeight / 2f);
        newY = Mathf.Clamp(newY, lowerClamp, upperClamp);
        Vector3 pos = thumb.position;
        pos.y = newY;
        thumb.position = pos;
        UpdateContentPosition();
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    Vector3 GetMouseWorldPosition()
    {
        // Use the thumb's z-distance from the camera to get an accurate world point.
        float zDist = Mathf.Abs(Camera.main.transform.position.z - thumb.position.z);
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDist));
    }

    void UpdateContentPosition()
    {
        // Determine normalized value: 0 when thumb's center is at trackTop and 1 when at trackBottom.
        float currentCenter = thumb.position.y;
        // Normalized scroll: 0 at top, 1 at bottom.
        float normalized = (trackTop - currentCenter - (thumbHeight / 2f)) / (trackRange - thumbHeight);
        normalized = Mathf.Clamp01(normalized);
        
        // Map normalized value to content offset.
        float contentOffset = normalized * scrollableContentRange + manualStopOffset;
        
        // Update content localPosition.y while preserving its X and Z.
        Vector3 newLocal = initialContentLocalPos;
        newLocal.y = initialContentLocalPos.y + contentOffset;
        content.localPosition = newLocal;
    }
}
