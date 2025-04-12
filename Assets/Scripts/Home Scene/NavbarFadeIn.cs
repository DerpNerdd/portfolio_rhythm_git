using UnityEngine;
using System.Collections;

public class NavbarFadeIn : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("Duration of the fade in (seconds)")]
    public float fadeDuration = 0.3f;
    [Tooltip("Target color for the navbar (including desired alpha)")]
    public Color targetColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Example gray

    private Material mat;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("NavbarFadeIn: No Renderer found on " + gameObject.name);
            return;
        }
        // Instantiate a new material instance for this object.
        mat = rend.material;
        Color col = mat.color;
        col.a = 0f;
        mat.color = col;
        Debug.Log("NavbarFadeIn: Material instantiated, initial alpha set to 0");
    }

    public void FadeIn()
    {
        Debug.Log("NavbarFadeIn: FadeIn called!");
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeInCoroutine()
    {
        float elapsed = 0f;
        Color initialColor = mat.color; // Should be alpha 0 at this point.
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            Color newColor = Color.Lerp(initialColor, targetColor, t);
            newColor.a = Mathf.Lerp(0f, targetColor.a, t);
            mat.color = newColor;
            yield return null;
        }
        mat.color = targetColor;
        Debug.Log("NavbarFadeIn: FadeIn completed, final color: " + mat.color);
    }
}
