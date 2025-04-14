using System.Collections;
using UnityEngine;

public class ResetAnalogClock : MonoBehaviour
{
    [Tooltip("Reference to the AnalogClock component that needs resetting.")]
    public AnalogClock clockScript;
    [Tooltip("Delay (in seconds) for how long the clock is disabled before re‑enabling.")]
    public float resetDelay = 0.01f; // Very short delay

    // This public method can be called once the pink circle is clicked,
    // or whenever you need to force the AnalogClock to reset.
    public void ResetClock()
    {
        if (clockScript != null)
        {
            StartCoroutine(ResetRoutine());
        }
        else
        {
            Debug.LogError("ResetAnalogClock: No AnalogClock reference assigned!");
        }
    }

    private IEnumerator ResetRoutine()
    {
        clockScript.enabled = false;
        yield return new WaitForSeconds(resetDelay);
        clockScript.enabled = true;
    }
}
