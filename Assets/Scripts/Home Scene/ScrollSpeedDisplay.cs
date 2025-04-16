using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Uncomment if you want to use TextMeshProUGUI instead

public class ScrollSpeedDisplay : MonoBehaviour
{
    [Tooltip("Reference to the Slider component (min: -60, max: 60).")]
    public Slider slider;
    
    [Tooltip("Reference to the Text component that shows the value (e.g., '0 ms').")]
    // public Text displayText;
    public TMP_Text displayText; // Use this if you prefer TextMeshProUGUI

    void Start()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(UpdateDisplay);
            // Update immediately for the default value
            UpdateDisplay(slider.value);
        }
        else
        {
            Debug.LogError("ScrollSpeedDisplay: Slider reference is missing.");
        }
    }


void UpdateDisplay(float value)
{
    // Format the value to 2 decimal places
    float floatValue = value;
    if (displayText != null)
    {
        displayText.text = floatValue.ToString("F1");
    }
    else
    {
        Debug.LogError("ScrollSpeedDisplay: Display Text reference is missing.");
    }
}
}