using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Uncomment if you want to use TextMeshProUGUI instead

public class SliderValueDisplay : MonoBehaviour
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
            Debug.LogError("SliderValueDisplay: Slider reference is missing.");
        }
    }

    void UpdateDisplay(float value)
    {
        // Convert the slider value to an integer to avoid decimals
        int intValue = Mathf.RoundToInt(value);
        if (displayText != null)
        {
            displayText.text = intValue.ToString() + " ms";
        }
        else
        {
            Debug.LogError("SliderValueDisplay: Display Text reference is missing.");
        }
    }
}
