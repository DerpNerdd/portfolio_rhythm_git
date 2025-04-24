// Assets/Scripts/Play Menu Scene/DiffSection2Controller.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DiffSection2Controller : MonoBehaviour
{
    [Header("HP Drain")]
    [Tooltip("Background bar (black)")]
    public RectTransform hpMainBar;
    [Tooltip("Fill bar (blue)")]
    public RectTransform hpSubBar;
    [Tooltip("Text showing value")]
    public TMP_Text hpText;

    [Header("Accuracy")]
    [Tooltip("Background bar (black)")]
    public RectTransform accMainBar;
    [Tooltip("Fill bar (blue)")]
    public RectTransform accSubBar;
    [Tooltip("Text showing value")]
    public TMP_Text accText;

    [Header("Star Difficulty")]
    [Tooltip("Background bar (black)")]
    public RectTransform starMainBar;
    [Tooltip("Fill bar (blue)")]
    public RectTransform starSubBar;
    [Tooltip("Text showing value")]
    public TMP_Text starText;

    [Header("Animation Settings")]
    [Tooltip("Duration of fill animation")]
    public float animationDuration = 0.2f;

    private Coroutine hpRoutine;
    private Coroutine accRoutine;
    private Coroutine starRoutine;

    private const float MAX_VALUE = 10f;

    /// <summary>
    /// Update all bars and texts smoothly.
    /// </summary>
    public void UpdateSection2(BeatmapInfo bm)
    {
        // Text updates
        hpText.text = bm.HPDrain.ToString("F1");
        accText.text = bm.Accuracy.ToString("F1");
        starText.text = bm.StarDifficulty.ToString("F2");

        // Animation ratios
        float hpRatio   = Mathf.Clamp01(bm.HPDrain / MAX_VALUE);
        float accRatio  = Mathf.Clamp01(bm.Accuracy / MAX_VALUE);
        float starRatio = Mathf.Clamp01(bm.StarDifficulty / MAX_VALUE);

        AnimateBar(hpMainBar, hpSubBar, hpRatio, ref hpRoutine);
        AnimateBar(accMainBar, accSubBar, accRatio, ref accRoutine);
        AnimateBar(starMainBar, starSubBar, starRatio, ref starRoutine);
    }

    private void AnimateBar(RectTransform mainBar, RectTransform subBar, float targetRatio, ref Coroutine routine)
    {
        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(BarRoutine(mainBar, subBar, targetRatio));
    }

    private IEnumerator BarRoutine(RectTransform mainBar, RectTransform subBar, float targetRatio)
    {
        float baseWidth   = mainBar.rect.width;
        float startWidth  = subBar.rect.width;
        float targetWidth = baseWidth * targetRatio;

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float newWidth = Mathf.Lerp(startWidth, targetWidth, t);
            subBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            yield return null;
        }

        // Ensure final width
        subBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
    }
}