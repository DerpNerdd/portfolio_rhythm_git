using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFadeIn : MonoBehaviour
{
    [Tooltip("Duration of the fade in (seconds)")]
    public float fadeDuration = 0.3f;
    [Tooltip("Target color for the button (including desired alpha)")]
    public Color targetColor = new Color(1f, 1f, 1f, 1f); // full white, or set to your desired color

    private Image imageComponent;
    
    void Start()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("UIFadeIn: No Image component found on " + gameObject.name);
            return;
        }
        // Ensure the button starts fully transparent.
        Color col = imageComponent.color;
        col.a = 0f;
        imageComponent.color = col;
    }
    
    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }
    
    IEnumerator FadeInCoroutine()
    {
        float elapsed = 0f;
        Color initialColor = imageComponent.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            Color newColor = Color.Lerp(initialColor, targetColor, t);
            imageComponent.color = newColor;
            yield return null;
        }
        imageComponent.color = targetColor;
    }
}
