// LinkTextTMPEffect.cs
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class LinkTextTMPEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Target TMP Text")]
    public TMP_Text targetText;

    [Header("Idle Twinkle")]
    public float idleTwinkleSpeed = 1f;
    public Color idleMinColor     = new Color(1,1,1,0.3f);
    public Color idleMaxColor     = new Color(1,1,1,0.8f);

    [Header("Hover")]
    public Color hoverColor       = Color.white;
    public float hoverScale       = 1.2f;

    [Header("Lerp Speeds")]
    public float colorLerpSpeed   = 8f;
    public float scaleLerpSpeed   = 10f;

    bool    isHovered;
    Vector3 baseScale;
    float   phase;

    void Awake()
    {
        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>();
        if (targetText == null)
            Debug.LogError("LinkTextTMPEffect needs a TMP_Text child.");

        var textRT = targetText.rectTransform;
        textRT.pivot     = new Vector2(0f, 1f);
        textRT.anchorMin = new Vector2(0f, 1f);
        textRT.anchorMax = new Vector2(0f, 1f);

        baseScale = targetText.transform.localScale;
        phase     = Random.Range(0f, Mathf.PI * 2f);
    }

    void OnEnable()  => TwinkleManager.Instance?.Register(this);
    void OnDisable() => TwinkleManager.Instance?.Unregister(this);

    public void OnPointerEnter(PointerEventData e) => isHovered = true;
    public void OnPointerExit (PointerEventData e) => isHovered = false;

    /// <summary>
    /// Called by the TwinkleManager every tick.
    /// </summary>
    public void Tick(float time)
    {
        // 1) Color
        Color target = isHovered
            ? hoverColor
            : Color.Lerp(idleMinColor, idleMaxColor,
                  (Mathf.Sin(time * idleTwinkleSpeed + phase) + 1f) * 0.5f);

        targetText.color = Color.Lerp(targetText.color, target, Time.deltaTime * colorLerpSpeed);

        // 2) Scale
        Vector3 s = isHovered ? baseScale * hoverScale : baseScale;
        targetText.transform.localScale =
            Vector3.Lerp(targetText.transform.localScale, s, Time.deltaTime * scaleLerpSpeed);
    }
}
