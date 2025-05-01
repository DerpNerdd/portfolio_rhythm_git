using TMPro;
using UnityEngine;

/// <summary>
/// Automatically clones the TextMeshProUGUI material and applies a neon-style blurred outline glow.
/// Attach this to any TextMeshProUGUI object.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextGlow : MonoBehaviour
{
    [Header("Face & Outline Settings")]
    public Color faceColor        = Color.white;
    public Color outlineColor     = new Color(0f, 0.5f, 1f, 1f);
    [Range(0f, 0.5f)]
    public float outlineWidth     = 0.1f;
    [Tooltip("Softness of the outline blur")]  
    [Range(0f, 1f)]
    public float outlineSoftness  = 0.2f;

    void Awake()
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        // Clone the shared material so we don't overwrite the asset
        var mat = Instantiate(tmp.fontMaterial);
        tmp.fontMaterial = mat;

        // Apply base face color
        mat.SetColor(ShaderUtilities.ID_FaceColor, faceColor);

        // Configure blurred outline as glow
        mat.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
        mat.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineWidth);
        mat.SetFloat(ShaderUtilities.ID_OutlineSoftness, outlineSoftness);
        mat.EnableKeyword("OUTLINE_ON");

        // Force a mesh update so extra padding is applied for the blur
        tmp.ForceMeshUpdate();
    }
}
