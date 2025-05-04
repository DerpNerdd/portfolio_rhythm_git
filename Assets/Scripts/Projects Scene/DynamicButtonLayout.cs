// DynamicButtonLayout.cs
using UnityEngine;
using UnityEngine.UI;      // LayoutRebuilder lives here
using TMPro;

[RequireComponent(typeof(LayoutElement))]
public class DynamicButtonLayout : MonoBehaviour
{
    [Header("Target TextMeshProUGUI")]
    public TMP_Text targetText;

    [Header("Padding (px)")]
    public float horizontalPadding = 16f;
    public float verticalPadding   = 8f;

    LayoutElement layoutElement;
    RectTransform parentRect;

    void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        parentRect    = transform.parent as RectTransform;

        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>();
        if (targetText == null)
            Debug.LogError("DynamicButtonLayout: No TMP_Text found!");
    }

    void Start()
    {
        UpdateLayout();
        // force the VerticalLayoutGroup to pick up your preferred sizes immediately
        Canvas.ForceUpdateCanvases();
        if (parentRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
    }

    public void UpdateLayout()
    {
        targetText.ForceMeshUpdate();
        float w = targetText.preferredWidth  + horizontalPadding;
        float h = targetText.preferredHeight + verticalPadding;
        layoutElement.preferredWidth  = w;
        layoutElement.preferredHeight = h;
    }
}
