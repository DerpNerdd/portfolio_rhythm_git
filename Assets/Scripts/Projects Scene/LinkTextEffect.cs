// LinkTextTMPEffect.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class LinkTextTMPEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Target TMP Text")]
    [Tooltip("Drag your TextMeshProUGUI child here")]
    public TMP_Text targetText;

    [Header("Idle Twinkle")]
    public float idleTwinkleSpeed = 1f;
    public Color idleMinColor     = new Color(1,1,1,0.3f);
    public Color idleMaxColor     = new Color(1,1,1,0.8f);

    [Header("Hover")]
    public Color hoverColor  = Color.white;
    public float hoverScale  = 1.2f;

    [Header("Animation Speeds")]
    public float colorLerpSpeed = 8f;
    public float scaleLerpSpeed = 10f;

    bool    isHovered;
    Vector3 baseScale;
    float   phase;

void Awake()
{
    if (targetText == null)
        targetText = GetComponentInChildren<TMP_Text>();

    if (targetText == null)
    {
        Debug.LogError("LinkTextTMPEffect requires a TextMeshProUGUI child!");
        enabled = false;
        return;
    }

    // **ONLY** snap the pivot X to the left edge
    var rt = targetText.rectTransform;
    rt.pivot = new Vector2(0f, 0.5f);

    baseScale = targetText.transform.localScale;
    phase     = Random.Range(0f, Mathf.PI * 2f);
}


    void Update()
    {
        // 1) Color tween
        Color desired = isHovered
            ? hoverColor
            : Color.Lerp(idleMinColor, idleMaxColor,
                (Mathf.Sin(Time.time * idleTwinkleSpeed + phase) + 1f) * 0.5f);

        targetText.color = Color.Lerp(targetText.color, desired, Time.deltaTime * colorLerpSpeed);

        // 2) Scale tween
        Vector3 targetScale = isHovered ? baseScale * hoverScale : baseScale;
        targetText.transform.localScale =
            Vector3.Lerp(targetText.transform.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData) => isHovered = true;
    public void OnPointerExit(PointerEventData eventData)  => isHovered = false;
}
