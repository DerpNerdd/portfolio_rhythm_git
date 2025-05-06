using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class AnalogClock : MonoBehaviour
{
    [Tooltip("Reference to the RectTransform of the hour hand.")]
    public RectTransform hourHand;
    [Tooltip("Reference to the RectTransform of the minute hand.")]
    public RectTransform minuteHand;
    [Tooltip("Reference to the RectTransform of the second hand.")]
    public RectTransform secondHand;

    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern double GetLocalTimeMillis();
    [DllImport("__Internal")]
    private static extern int GetTimezoneOffsetMinutes();
    #endif

    void Update()
    {
        UpdateClock();
    }

    void UpdateClock()
    {
        DateTime now;

        #if UNITY_WEBGL && !UNITY_EDITOR
            // 1) Get UTC ms from JS
            double utcMs = GetLocalTimeMillis();
            // 2) Minutes local→UTC
            int offsetMin = GetTimezoneOffsetMinutes();
            // 3) Compute local ms
            double localMs = utcMs - offsetMin * 60_000.0;
            // 4) Build DateTime directly in local
            now = new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc)
                      .AddMilliseconds(localMs);
        #else
            now = DateTime.Now;
        #endif

        // fractional components for smooth motion
        float hour   = (now.Hour % 12) + now.Minute / 60f + now.Second / 3600f;
        float minute = now.Minute + now.Second / 60f + now.Millisecond / 60000f;
        float second = now.Second + now.Millisecond / 1000f;

        float hourAngle   = hour   * 30f; // 360/12
        float minuteAngle = minute *  6f; // 360/60
        float secondAngle = second *  6f;

        if (hourHand   != null) hourHand.localRotation   = Quaternion.Euler(0f,0f,-hourAngle);
        if (minuteHand != null) minuteHand.localRotation = Quaternion.Euler(0f,0f,-minuteAngle);
        if (secondHand != null) secondHand.localRotation = Quaternion.Euler(0f,0f,-secondAngle);
    }
}
