using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles Success Rate, User Rating, and Rating Spread bars & texts with animations.
/// </summary>
public class DiffSection3Controller : MonoBehaviour
{
    [Header("Success Rate")]
    [Tooltip("Background bar (black)")]
    public RectTransform successMainBar;
    [Tooltip("Fill bar (blue)")]
    public RectTransform successSubBar;
    [Tooltip("Percentage text")]
    public TMP_Text successRatePercentage;

    [Header("User Rating")]
    [Tooltip("Background bar (black)")]
    public RectTransform userMainBar;
    [Tooltip("Fill bar (green)")]
    public RectTransform userSubBar;
    [Tooltip("Red percentage text")]
    public TMP_Text userRateRedText;
    [Tooltip("Green percentage text")]
    public TMP_Text userRateGreenText;

    [Header("Rating Spread Recs")]
    public RectTransform darkRedRec;
    public RectTransform redRec;
    public RectTransform darkOrgRec;
    public RectTransform orgRec;
    public RectTransform darkYellowRec;
    public RectTransform yellowRec;
    public RectTransform darkGreenRec;
    public RectTransform greenRec;
    public RectTransform lightGreenRec;
    public RectTransform lightestGreenRec;

    [Header("Rec Height Settings")]
    [Tooltip("Minimum bar height in pixels")]
    public float minRecHeight = 3f;
    [Tooltip("Maximum bar height in pixels")]
    public float maxRecHeight = 60f;

    [Header("Animation Settings")]
    [Tooltip("Duration of fill animation in seconds")]
    public float animationDuration = 0.2f;

    private Coroutine successRoutine;
    private Coroutine userRoutine;

    private Coroutine darkRedRoutine;
    private Coroutine redRoutine;
    private Coroutine darkOrgRoutine;
    private Coroutine orgRoutine;
    private Coroutine darkYellowRoutine;
    private Coroutine yellowRoutine;
    private Coroutine darkGreenRoutine;
    private Coroutine greenRoutine;
    private Coroutine lightGreenRoutine;
    private Coroutine lightestGreenRoutine;

    private const float MAX_PERCENT = 100f;

    /// <summary>
    /// Update section 3: success rate, user rating, and rating spread.
    /// </summary>
    public void UpdateSection3(BeatmapInfo bm)
    {
        // 1) Success Rate
        float sr = bm.SuccessRate;
        successRatePercentage.text = sr.ToString("F1") + "%";
        AnimateBar(successMainBar, successSubBar, sr / MAX_PERCENT, ref successRoutine);

        // 2) User Rating
        float urRed   = bm.UserRating.Red;
        float urGreen = bm.UserRating.Green;
        userRateRedText.text   = urRed.ToString("F1") + "%";
        userRateGreenText.text = urGreen.ToString("F1") + "%";
        AnimateBar(userMainBar, userSubBar, urGreen / MAX_PERCENT, ref userRoutine);

        // 3) Rating Spread
        SetAnimatedRec(darkRedRec,       bm.RatingSpread.DarkRedRec,      ref darkRedRoutine);
        SetAnimatedRec(redRec,           bm.RatingSpread.RedRec,          ref redRoutine);
        SetAnimatedRec(darkOrgRec,       bm.RatingSpread.DarkOrgRec,      ref darkOrgRoutine);
        SetAnimatedRec(orgRec,           bm.RatingSpread.OrgRec,          ref orgRoutine);
        SetAnimatedRec(darkYellowRec,    bm.RatingSpread.DarkYellowRec,   ref darkYellowRoutine);
        SetAnimatedRec(yellowRec,        bm.RatingSpread.YellowRec,       ref yellowRoutine);
        SetAnimatedRec(darkGreenRec,     bm.RatingSpread.DarkGreenRec,    ref darkGreenRoutine);
        SetAnimatedRec(greenRec,         bm.RatingSpread.GreenRec,        ref greenRoutine);
        SetAnimatedRec(lightGreenRec,    bm.RatingSpread.LightGreenRec,   ref lightGreenRoutine);
        SetAnimatedRec(lightestGreenRec, bm.RatingSpread.LightestGreenRec, ref lightestGreenRoutine);
    }

    private void AnimateBar(RectTransform mainBar, RectTransform subBar, float targetRatio, ref Coroutine routine)
    {
        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(BarRoutine(mainBar, subBar, targetRatio));
    }

    private IEnumerator BarRoutine(RectTransform mainBar, RectTransform subBar, float targetRatio)
    {
        float baseW   = mainBar.rect.width;
        float startW  = subBar.rect.width;
        float targetW = baseW * Mathf.Clamp01(targetRatio);
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float newW = Mathf.Lerp(startW, targetW, t);
            subBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newW);
            yield return null;
        }
        subBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetW);
    }

    private void SetAnimatedRec(RectTransform rec, float ratio, ref Coroutine routine)
    {
        if (rec == null) return;
        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(RecRoutine(rec, ratio));
    }

    private IEnumerator RecRoutine(RectTransform rec, float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        float startH  = rec.rect.height;
        float targetH = Mathf.Lerp(minRecHeight, maxRecHeight, ratio);
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float newH = Mathf.Lerp(startH, targetH, t);
            rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newH);
            yield return null;
        }
        rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetH);
    }
}