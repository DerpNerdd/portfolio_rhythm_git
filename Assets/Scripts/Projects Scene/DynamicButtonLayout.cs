using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(LayoutElement))]
public class DynamicButtonLayout : MonoBehaviour
{
    [Header("Target TextMeshProUGUI")]
    public TMP_Text targetText;

    [Header("Padding (px)")]
    public float horizontalPadding = 16f;
    public float verticalPadding   = 8f;

    LayoutElement   layoutElement;
    RectTransform   rt;

    void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        rt            = GetComponent<RectTransform>();

        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>();
        if (targetText == null)
            Debug.LogError("DynamicButtonLayout: No TMP_Text found!");
    }

    void Start()
    {
        UpdateLayout();
    }

public void UpdateLayout()
{
    targetText.ForceMeshUpdate();
    float w = targetText.preferredWidth  + horizontalPadding;
    float h = targetText.preferredHeight + verticalPadding;

    layoutElement.minWidth        = w;
    layoutElement.preferredWidth  = w;
    layoutElement.minHeight       = h;
    layoutElement.preferredHeight = h;

    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   h);

    // ensure the parent VerticalLayoutGroup rebuilds immediately
    Canvas.ForceUpdateCanvases();
    var parentRect = rt.parent as RectTransform;
    LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
}
}
