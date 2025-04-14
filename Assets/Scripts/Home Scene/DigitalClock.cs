using UnityEngine;
using TMPro;
using System;

public class DigitalClock : MonoBehaviour
{
    // We now automatically retrieve the TMP_Text component from the same GameObject.
    private TMP_Text digitalClockText;

    void Awake()
    {
        digitalClockText = GetComponent<TMP_Text>();
        if (digitalClockText == null)
        {
            Debug.LogError("DigitalClock: No TMP_Text component found on this GameObject.");
        }
    }

    void Update()
    {
        DateTime now = DateTime.Now;
        if (digitalClockText != null)
        {
            digitalClockText.text = now.ToString("hh:mm:ss tt");
        }
    }
}
