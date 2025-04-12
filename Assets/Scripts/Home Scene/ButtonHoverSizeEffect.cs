using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ButtonHoverSizeEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector2 originalSize;
    [Tooltip("Multiplier for the height increase when hovered (e.g. 1.1 means 10% taller)")]
    public float heightMultiplier = 1.1f;
    [Tooltip("Duration of the size animation (in seconds)")]
    public float animationDuration = 0.2f;

    private Coroutine currentAnimation;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalSize = rectTransform.sizeDelta;
            // Set the pivot to bottom center so that only the top moves when height increases.
            rectTransform.pivot = new Vector2(0.5f, 0f);
        }
        else
        {
            Debug.LogError("ButtonHoverSizeEffect: No RectTransform found on " + gameObject.name);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 targetSize = new Vector2(originalSize.x, originalSize.y * heightMultiplier);
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateSize(rectTransform.sizeDelta, targetSize, animationDuration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateSize(rectTransform.sizeDelta, originalSize, animationDuration));
    }

    IEnumerator AnimateSize(Vector2 startSize, Vector2 endSize, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // Lerp the size.
            rectTransform.sizeDelta = Vector2.Lerp(startSize, endSize, t);
            yield return null;
        }
        rectTransform.sizeDelta = endSize;
        currentAnimation = null;
    }
}
