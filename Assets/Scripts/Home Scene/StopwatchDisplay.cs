using UnityEngine;
using TMPro;

public class StopwatchDisplay : MonoBehaviour
{
    private TMP_Text stopwatchText;
    private float elapsedTime = 0f;

    void Awake()
    {
        // Automatically grab the TMP_Text component on this GameObject
        stopwatchText = GetComponent<TMP_Text>();
        if(stopwatchText == null)
        {
            Debug.LogError("StopwatchDisplay: No TMP_Text component found on this GameObject.");
        }
    }

    void Update()
    {
        // Accumulate elapsed time. This resets every time the game stops/restarts.
        elapsedTime += Time.deltaTime;

        // Convert elapsed time (in seconds) to hours, minutes, and seconds.
        int hours = (int)(elapsedTime / 3600f);
        int minutes = (int)((elapsedTime % 3600f) / 60f);
        int seconds = (int)(elapsedTime % 60f);

        // Set the text to "running hh:mm:ss"
        stopwatchText.text = string.Format("running {0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }
}
