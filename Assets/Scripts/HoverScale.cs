using UnityEngine;

public class HoverScale : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign your PinkCircleQuad (the pink circle object) here. It must have a collider attached.")]
    public GameObject pinkCircle;
    [Tooltip("Assign the parent GameObject of your visualizer bars here (the container from AudioVisualizer).")]
    public GameObject barsParent;

    [Header("Scaling Settings")]
    [Tooltip("Multiplier for how much to scale up when hovered (e.g., 1.1 for a 10% increase).")]
    public float hoverScaleMultiplier = 1.1f;
    [Tooltip("Speed for the scale transition (Lerp factor).")]
    public float smoothSpeed = 5f;

    // Store the original scales (we only want to modify x and y)
    private Vector3 originalCircleScale;
    private Vector3 originalBarsScale;

    // Reference to the main camera for raycasting
    private Camera mainCamera;

    void Start()
    {
        // Ensure the referenced objects have been assigned.
        if (pinkCircle == null)
        {
            Debug.LogError("HoverScale: PinkCircle reference is not assigned!");
        }
        else
        {
            originalCircleScale = pinkCircle.transform.localScale;
        }

        if (barsParent == null)
        {
            Debug.LogError("HoverScale: BarsParent reference is not assigned!");
        }
        else
        {
            originalBarsScale = barsParent.transform.localScale;
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("HoverScale: No Main Camera found in the scene!");
        }
    }

    void Update()
    {
        bool isHovered = false;
        // Perform a raycast from the mouse position.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject == pinkCircle)
            {
                isHovered = true;
            }
        }

        // Determine target scales only for the x and y components.
        float targetScaleFactor = isHovered ? hoverScaleMultiplier : 1f;
        Vector3 targetCircleScale = new Vector3(originalCircleScale.x * targetScaleFactor,
                                                originalCircleScale.y * targetScaleFactor,
                                                originalCircleScale.z); // Z remains unchanged

        Vector3 targetBarsScale = new Vector3(originalBarsScale.x * targetScaleFactor,
                                              originalBarsScale.y * targetScaleFactor,
                                              originalBarsScale.z); // Z remains unchanged

        // Smoothly Lerp the pink circle's x and y scale towards the target.
        if (pinkCircle != null)
        {
            Vector3 currentCircleScale = pinkCircle.transform.localScale;
            pinkCircle.transform.localScale = new Vector3(
                Mathf.Lerp(currentCircleScale.x, targetCircleScale.x, Time.deltaTime * smoothSpeed),
                Mathf.Lerp(currentCircleScale.y, targetCircleScale.y, Time.deltaTime * smoothSpeed),
                originalCircleScale.z  // Maintain the original z-scale
            );
        }

        // Smoothly Lerp the barsParent's x and y scale towards the target.
        if (barsParent != null)
        {
            Vector3 currentBarsScale = barsParent.transform.localScale;
            barsParent.transform.localScale = new Vector3(
                Mathf.Lerp(currentBarsScale.x, targetBarsScale.x, Time.deltaTime * smoothSpeed),
                Mathf.Lerp(currentBarsScale.y, targetBarsScale.y, Time.deltaTime * smoothSpeed),
                originalBarsScale.z  // Maintain the original z-scale
            );
        }
    }
}
