using TMPro;
using UnityEngine;

/// <summary>
/// Automatically clones the TextMeshProUGUI material and applies a neon-outline + glow effect.
/// Attach this to any TextMeshProUGUI object.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextGlow : MonoBehaviour
{
    [Header("Face & Outline Settings")]
    public Color faceColor        = Color.white;
    public Color outlineColor     = new Color(0f,0.5f,1f,1f);
    [Range(0f,0.5f)]
    public float outlineWidth     = 0.1f;

    [Header("Glow (Underlay) Settings)")]
    public Color glowColor        = new Color(0f,0.5f,1f,1f);
    [Tooltip("How far the glow extends beyond the text shape (dilate) in SDF units")]
    public float glowDilate       = 0.5f;
    [Tooltip("Softness of the glow edge, higher = more diffuse")]
    public float glowSoftness     = 10f;

    void Awake()
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        // Clone the shared material so we don't overwrite the asset
        var mat = Instantiate(tmp.fontMaterial);
        tmp.fontMaterial = mat;

        // Face color
        mat.SetColor(ShaderUtilities.ID_FaceColor, faceColor);

        // Outline settings
        mat.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineWidth);
        mat.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
        mat.EnableKeyword("OUTLINE_ON");

        // Underlay (glow) settings
        mat.SetColor(ShaderUtilities.ID_UnderlayColor, glowColor);
        mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0f);
        mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, 0f);
        mat.SetFloat(ShaderUtilities.ID_UnderlayDilate, glowDilate);
        mat.SetFloat(ShaderUtilities.ID_UnderlaySoftness, glowSoftness);
        mat.EnableKeyword("UNDERLAY_ON");
    }
}
