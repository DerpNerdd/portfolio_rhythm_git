using UnityEngine;

public class BeatBounce : MonoBehaviour
{
    [Header("Audio Settings")]
    // Assign your audio source (the one playing the music) in the Inspector
    public AudioSource audioSource;
    // Number of spectrum data samples to get (typical values: 64, 128, 256, etc.)
    public int spectrumSize = 128;

    [Header("Bounce Settings")]
    // Multiplier for scaling based on the audio amplitude (tweak as needed)
    public float scaleMultiplier = 5f;
    // How quickly the object smooths toward the target scale
    public float smoothSpeed = 4f;

    private Vector3 initialScale;
    private float[] spectrumData;

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned on " + gameObject.name);
        }
        // Store the initial scale of the object so we scale relative to it.
        initialScale = transform.localScale;
        // Prepare the array to hold the spectrum data.
        spectrumData = new float[spectrumSize];
    }

    void Update()
    {
        if (audioSource == null) return;

        // Fill the spectrumData array with audio spectrum information.
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);

        // Compute the average amplitude from the spectrum.
        float sum = 0f;
        for (int i = 0; i < spectrumSize; i++)
        {
            sum += spectrumData[i];
        }
        float averageAmplitude = sum / spectrumSize;

        // Calculate a new scale factor based on the average amplitude.
        // Increase the scale by (1 + averageAmplitude * scaleMultiplier)
        Vector3 targetScale = initialScale * (1f + averageAmplitude * scaleMultiplier);

        // Smoothly transition to the new scale.
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
    }
}
