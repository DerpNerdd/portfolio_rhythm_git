// Assets/Scripts/Play Menu Scene/HoverScaler.cs
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Scale when hovered")]
    public Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1f);
    [Tooltip("Animation duration in seconds")]
    public float duration = 0.1f;

    private Vector3 originalScale;
    private Coroutine currentRoutine;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScale(hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScale(originalScale);
    }

    private void StartScale(Vector3 target)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ScaleRoutine(target));
    }

    private IEnumerator ScaleRoutine(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        transform.localScale = target;
        currentRoutine = null;
    }
}
