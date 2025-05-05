using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Runtime.InteropServices;

[DisallowMultipleComponent]
public class DigitalClock : MonoBehaviour
{
    [Tooltip("Drag your TextMeshProUGUI component here (optional if on same GameObject)")]
    public TMP_Text digitalClockText;

    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern double GetLocalTimeMillis();

    [DllImport("__Internal")]
    private static extern int GetTimezoneOffsetMinutes();
    #endif

    void Awake()
    {
        if (digitalClockText == null)
            digitalClockText = GetComponent<TMP_Text>();

        if (digitalClockText == null)
            Debug.LogError("DigitalClock: No TMP_Text assigned or found on this GameObject.");
    }

    void Start()
    {
        StartCoroutine(UpdateClock());
    }

    private IEnumerator UpdateClock()
    {
        while (true)
        {
            DateTime now;

            #if UNITY_WEBGL && !UNITY_EDITOR
                // 1) raw UTC ms from JS
                double utcMs = GetLocalTimeMillis();
                // 2) JS offset in minutes (local → UTC)
                int offsetMin = GetTimezoneOffsetMinutes();
                // 3) convert to local ms
                double localMs = utcMs - offsetMin * 60_000.0;
                // 4) build a DateTime from that
                now = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                          .AddMilliseconds(localMs);
            #else
                // Editor & Standalone fallback
                now = DateTime.Now;
            #endif

            // update the visible text
            digitalClockText.text = now.ToString("hh:mm:ss tt");

            // wait until the next full second
            float delay = 1f - (now.Millisecond / 1000f);
            yield return new WaitForSecondsRealtime(Mathf.Max(delay, 0.01f));
        }
    }
}
