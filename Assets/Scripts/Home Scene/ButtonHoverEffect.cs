using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image btnImage;
    [Tooltip("The default (base) color for the button once faded in. Set this in the Inspector.")]
    public Color defaultColor = new Color(1, 1, 1, 1);
    [Tooltip("Color when hovered (for example, a lighter version).")]
    public Color hoverColor = new Color(1, 1, 1, 1);

    // This flag prevents the hover effect until we're ready.
    public bool hoverEnabled = false;

    void Start()
    {
        btnImage = GetComponent<Image>();
        if (btnImage == null)
        {
            Debug.LogError("ButtonHoverEffect: No Image component found on " + gameObject.name);
        }
        // Do NOT set the color here so as not to override the fade in.
    }

    public void EnableHover()
    {
        hoverEnabled = true;
        // Optionally, ensure the image is set to the defaultColor.
        if (btnImage != null)
        {
            btnImage.color = defaultColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!hoverEnabled || btnImage == null)
            return;
        btnImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!hoverEnabled || btnImage == null)
            return;
        btnImage.color = defaultColor;
    }
}
