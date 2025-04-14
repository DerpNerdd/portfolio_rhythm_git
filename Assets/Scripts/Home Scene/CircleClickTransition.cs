using System.Collections;
using UnityEngine;

public class CircleClickTransition : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the PinkCircleQuad (this object). Ensure this object has a 2D collider for OnMouseDown to work.")]
    public Transform pinkCircle;
    [Tooltip("Reference to the AudioVisualizer (the object containing the visualizer bars).")]
    public Transform barsGroup;
    [Tooltip("Optional: Reference to the HoverScale script controlling the pink circle and visualizer bars. It will be updated and then disabled after transition.")]
    public HoverScale hoverScaleScript;
    [Tooltip("Reference to the GrayNavbar object (with NavbarFadeIn attached) that will fade in.")]
    public NavbarFadeIn navbar;
    [Tooltip("Reference to the parent GameObject that holds all the buttons which should fade in. Ensure it has a CanvasGroup set to alpha 0 and not interactable initially.")]
    public Transform buttonsGroup;
    // Public field for the 3D top navbar (with TopBarSlideIn attached).
    [Tooltip("Reference to the 3D Top Navbar (with TopBarSlideIn attached).")]
    public TopBarSlideIn3D topNavbar;

    // NEW: Reference to the AnalogClock component that is hidden until the pink circle is clicked.
    [Tooltip("Reference to the AnalogClock script that needs to be reset (disable/enable) when the pink circle is clicked.")]
    public AnalogClock analogClockScript;

    [Header("Transition Settings")]
    [Tooltip("Target x,y position for both groups after click (their original z-values are preserved).")]
    public Vector2 targetPositionXY = new Vector2(-2.55f, -0.2f);
    [Tooltip("Scale multiplier relative to each object's original scale. For example, if PinkCircleQuad's original scale is 5 and you want it to be 3, use 0.6.")]
    public float circleScaleMultiplier = 0.6f;
    [Tooltip("For the visualizer bars, use 1 to keep the same size, or adjust as needed.")]
    public float barsScaleMultiplier = 1f;
    [Tooltip("Duration of the transition (in seconds). Also set the fade durations in the fade scripts to this value.")]
    public float transitionDuration = 1f;

    private bool hasTransitioned = false;

    void OnMouseDown()
    {
        Debug.Log("CircleClickTransition: OnMouseDown triggered on " + gameObject.name);
        if (!hasTransitioned)
        {
            hasTransitioned = true;

            // Disable HoverScale to prevent interference.
            if (hoverScaleScript != null)
            {
                hoverScaleScript.enabled = false;
                Debug.Log("CircleClickTransition: HoverScale disabled.");
            }

            // Start the navbar fade concurrently.
            if (navbar != null)
            {
                navbar.FadeIn();
                Debug.Log("CircleClickTransition: NavbarFadeIn.FadeIn() called concurrently.");
            }
            
            // Start the top navbar slide in concurrently.
            if (topNavbar != null)
            {
                topNavbar.SlideIn();
                Debug.Log("CircleClickTransition: TopBarSlideIn3D.SlideIn() called concurrently.");
            }
            else
            {
                Debug.LogError("CircleClickTransition: topNavbar is not assigned in the inspector!");
            }

            // NEW: Quickly disable and then enable the AnalogClock script to force it to update.
            if (analogClockScript != null)
            {
                StartCoroutine(ResetClockRoutine());
            }
            else
            {
                Debug.LogWarning("CircleClickTransition: analogClockScript is not assigned in the inspector!");
            }

            // Trigger fade in for each button in the buttonsGroup.
            if (buttonsGroup != null)
            {
                CanvasGroup cg = buttonsGroup.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = 0; 
                    cg.interactable = false;
                    cg.blocksRaycasts = false;
                }

                foreach (Transform child in buttonsGroup)
                {
                    var fadeIn = child.GetComponent<UIFadeIn>();
                    if (fadeIn != null)
                    {
                        fadeIn.FadeIn();
                        Debug.Log("CircleClickTransition: UIFadeIn triggered on " + child.name);
                    }

                    UIFadeInGraphic[] graphicFades = child.GetComponentsInChildren<UIFadeInGraphic>();
                    foreach (var gf in graphicFades)
                    {
                        gf.FadeIn();
                        Debug.Log("CircleClickTransition: UIFadeInGraphic triggered on " + gf.gameObject.name);
                    }
                }
            }

            StartCoroutine(DoTransition());
        }
    }

    IEnumerator DoTransition()
    {
        Vector3 initialPosCircle = pinkCircle.position;
        Vector3 initialPosBars = barsGroup.position;
        Vector3 initialScaleCircle = pinkCircle.localScale;
        Vector3 initialScaleBars = barsGroup.localScale;

        Vector3 targetScaleCircle = new Vector3(
            initialScaleCircle.x * circleScaleMultiplier,
            initialScaleCircle.y * circleScaleMultiplier,
            initialScaleCircle.z
        );
        Vector3 targetScaleBars = new Vector3(
            initialScaleBars.x * barsScaleMultiplier,
            initialScaleBars.y * barsScaleMultiplier,
            initialScaleBars.z
        );

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float easeT = t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;

            pinkCircle.position = new Vector3(
                Mathf.Lerp(initialPosCircle.x, targetPositionXY.x, easeT),
                Mathf.Lerp(initialPosCircle.y, targetPositionXY.y, easeT),
                initialPosCircle.z
            );
            barsGroup.position = new Vector3(
                Mathf.Lerp(initialPosBars.x, targetPositionXY.x, easeT),
                Mathf.Lerp(initialPosBars.y, targetPositionXY.y, easeT),
                initialPosBars.z
            );

            float newScaleX_Circle = Mathf.Lerp(initialScaleCircle.x, targetScaleCircle.x, easeT);
            float newScaleY_Circle = Mathf.Lerp(initialScaleCircle.y, targetScaleCircle.y, easeT);
            pinkCircle.localScale = new Vector3(newScaleX_Circle, newScaleY_Circle, initialScaleCircle.z);

            float newScaleX_Bars = Mathf.Lerp(initialScaleBars.x, targetScaleBars.x, easeT);
            float newScaleY_Bars = Mathf.Lerp(initialScaleBars.y, targetScaleBars.y, easeT);
            barsGroup.localScale = new Vector3(newScaleX_Bars, newScaleY_Bars, initialScaleBars.z);

            yield return null;
        }
        pinkCircle.position = new Vector3(targetPositionXY.x, targetPositionXY.y, initialPosCircle.z);
        barsGroup.position = new Vector3(targetPositionXY.x, targetPositionXY.y, initialPosBars.z);
        pinkCircle.localScale = targetScaleCircle;
        barsGroup.localScale = targetScaleBars;
        Debug.Log("CircleClickTransition: Transition complete.");

        BeatBounce bounce = pinkCircle.GetComponent<BeatBounce>();
        if (bounce != null)
        {
            bounce.UpdateInitialScale();
            Debug.Log("CircleClickTransition: BeatBounce initial scale updated.");
        }

        if (buttonsGroup != null)
        {
            CanvasGroup cg = buttonsGroup.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1;
                cg.interactable = true;
                cg.blocksRaycasts = true;
                Debug.Log("CircleClickTransition: Buttons CanvasGroup set to interactable.");
            }
        }
    }

    IEnumerator ResetClockRoutine()
    {
        // Quickly disable and then enable the AnalogClock script.
        analogClockScript.enabled = false;
        yield return new WaitForSeconds(0.01f);
        analogClockScript.enabled = true;
    }
}
