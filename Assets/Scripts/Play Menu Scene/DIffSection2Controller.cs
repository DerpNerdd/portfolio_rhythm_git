// Assets/Scripts/Play Menu Scene/DiffSection2Controller.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiffSection2Controller : MonoBehaviour
{
    [Header("HP Drain")]
    [Tooltip("Background bar (black)")]
    public RectTransform hpMainBar;
    [Tooltip("Fill bar (blue)")]
    public RectTransform hpSubBar;
    [Tooltip("Text showing value")]
    public TMP_Text      hpText;

    [Header("Accuracy")]
    [Tooltip("Background bar (black)")]
    public RectTransform accMainBar;
    [Tooltip("Fill bar (blue)")]
    public RectTransform accSubBar;
    [Tooltip("Text showing value")]
    public TMP_Text      accText;

    [Header("Star Difficulty")]
    [Tooltip("Background bar (black)")]
    public RectTransform starMainBar;
    [Tooltip("Fill bar (blue)")]
    public RectTransform starSubBar;
    [Tooltip("Text showing value")]
    public TMP_Text      starText;

    const float k_MaxValue = 10f;

    /// <summary>
    /// Call this with the beatmap info to update bars & texts.
    /// </summary>
public void UpdateSection2(BeatmapInfo bm)
{
    const float MAX = 10f;

    UpdateBar(hpMainBar,   hpSubBar,   bm.HPDrain,       MAX);
    hpText.text = bm.HPDrain.ToString("F1");

    UpdateBar(accMainBar,  accSubBar,  bm.Accuracy,      MAX);
    accText.text = bm.Accuracy.ToString("F1");

    UpdateBar(starMainBar, starSubBar, bm.StarDifficulty, MAX);
    starText.text = bm.StarDifficulty.ToString("F2");
}

void UpdateBar(RectTransform mainBar, RectTransform subBar, float value, float max)
{
    float ratio = Mathf.Clamp01(value / max);
    float fillW = mainBar.rect.width * ratio;
    subBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fillW);
}

}
