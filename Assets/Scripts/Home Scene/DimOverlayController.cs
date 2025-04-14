using System.Collections;
using UnityEngine;

public class DimOverlayController : MonoBehaviour
{
    [Tooltip("The target color when the overlay is active. Adjust the alpha (opacity) as needed (e.g. 0.5 for 50% opacity).")]
    public Color activeColor = new Color(0, 0, 0, 0.5f);
    [Tooltip("Duration of the fade in/out animation in seconds.")]
    public float fadeDuration = 0.5f;

    private Material overlayMaterial;
    private Color transparentColor;
    private bool isFading = false;

    void Awake()
    {
        // This script assumes the GameObject has a Renderer (for example, a MeshRenderer)
        Renderer rend = GetComponent<Renderer>();
        if(rend != null)
        {
            // Create an instance of the material to avoid modifying the shared asset.
            overlayMaterial = rend.material;
            // Start fully transparent.
            transparentColor = new Color(activeColor.r, activeColor.g, activeColor.b, 0f);
            overlayMaterial.color = transparentColor;
        }
        else
        {
            Debug.LogError("DimOverlayController: No Renderer found on the overlay GameObject.");
        }
    }

    // Call this method to dim the background.
    public void FadeIn()
    {
        if (overlayMaterial != null && !isFading)
        {
            StartCoroutine(FadeMaterial(overlayMaterial.color, activeColor));
        }
    }

    // Call this method to clear the dimming.
    public void FadeOut()
    {
        if (overlayMaterial != null && !isFading)
        {
            StartCoroutine(FadeMaterial(overlayMaterial.color, transparentColor));
        }
    }

    IEnumerator FadeMaterial(Color from, Color to)
    {
        isFading = true;
        float elapsed = 0f;
        while(elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            overlayMaterial.color = Color.Lerp(from, to, t);
            yield return null;
        }
        overlayMaterial.color = to;
        isFading = false;
    }
}
