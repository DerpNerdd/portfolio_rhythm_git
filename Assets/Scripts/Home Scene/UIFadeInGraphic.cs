using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Graphic))]
public class UIFadeInGraphic : MonoBehaviour
{
    [Tooltip("Duration of the fade in (seconds)")]
    public float fadeDuration = 0.3f;
    [Tooltip("Target color for the graphic (including desired alpha)")]
    public Color targetColor = new Color(1f, 1f, 1f, 1f);
    
    private Graphic graphicComp;
    
    void Start()
    {
        graphicComp = GetComponent<Graphic>();
        if (graphicComp == null)
        {
            Debug.LogError("UIFadeInGraphic: No Graphic component found on " + gameObject.name);
            return;
        }
        // Start fully transparent.
        Color col = graphicComp.color;
        col.a = 0f;
        graphicComp.color = col;
    }
    
    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }
    
    IEnumerator FadeInCoroutine()
    {
        float elapsed = 0f;
        Color initialColor = graphicComp.color; // Should be alpha 0 at start.
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            Color newColor = Color.Lerp(initialColor, targetColor, t);
            graphicComp.color = newColor;
            yield return null;
        }
        graphicComp.color = targetColor;
    }
}
