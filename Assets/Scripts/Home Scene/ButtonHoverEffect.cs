using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image btnImage;
    private Color originalColor;
    [Tooltip("Color when hovered (e.g. a lighter version)")]
    public Color hoverColor = new Color(1f, 1f, 1f, 1f);
    
    void Start()
    {
        btnImage = GetComponent<Image>();
        if (btnImage != null)
        {
            originalColor = btnImage.color;
        }
        else
        {
            Debug.LogError("ButtonHoverEffect: No Image component on " + gameObject.name);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btnImage != null)
        {
            btnImage.color = hoverColor;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (btnImage != null)
        {
            btnImage.color = originalColor;
        }
    }
}
