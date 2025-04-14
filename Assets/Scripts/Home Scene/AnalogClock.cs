using UnityEngine;
using System;

public class AnalogClock : MonoBehaviour
{
    [Tooltip("Reference to the RectTransform of the hour hand.")]
    public RectTransform hourHand;
    [Tooltip("Reference to the RectTransform of the minute hand.")]
    public RectTransform minuteHand;
    [Tooltip("Reference to the RectTransform of the second hand.")]
    public RectTransform secondHand;

    void Update()
    {
        UpdateClock();
    }

    void UpdateClock()
    {
        DateTime now = DateTime.Now;

        // Calculate fractional values for smooth motion.
        float hour = (now.Hour % 12) + now.Minute / 60f;
        float minute = now.Minute + now.Second / 60f;
        float second = now.Second + now.Millisecond / 1000f;

        // Each hour represents 30 degrees (360/12), and each minute/second 6 degrees (360/60).
        float hourAngle = hour * 30f;
        float minuteAngle = minute * 6f;
        float secondAngle = second * 6f;

        // Set local rotations (negative to account for clockwise movement in UI).
        if(hourHand != null)
            hourHand.localRotation = Quaternion.Euler(0f, 0f, -hourAngle);
        if(minuteHand != null)
            minuteHand.localRotation = Quaternion.Euler(0f, 0f, -minuteAngle);
        if(secondHand != null)
            secondHand.localRotation = Quaternion.Euler(0f, 0f, -secondAngle);
    }
}
