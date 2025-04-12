using UnityEngine;

public class BeatBounce : MonoBehaviour
{
    [Header("Audio Settings")]
    // Assign your audio source in the Inspector (or leave empty or disabled to auto-find).
    public AudioSource audioSource;
    public int spectrumSize = 128;

    [Header("Bounce Settings")]
    public float scaleMultiplier = 5f;
    public float smoothSpeed = 4f;

    private Vector3 initialScale;
    private float[] spectrumData;

    void Start()
    {
        // If the audio source is null or disabled, try to find the persistent MusicPlayer's AudioSource.
        if (audioSource == null || !audioSource.enabled)
        {
            if (MusicPlayer.instance != null)
            {
                audioSource = MusicPlayer.instance.GetComponent<AudioSource>();
            }
            else
            {
                Debug.LogError("No persistent MusicPlayer found for BeatBounce on " + gameObject.name);
            }
        }

        initialScale = transform.localScale;
        spectrumData = new float[spectrumSize];
    }

    void Update()
    {
        if (audioSource == null) return;

        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);
        float sum = 0f;
        for (int i = 0; i < spectrumSize; i++)
        {
            sum += spectrumData[i];
        }
        float averageAmplitude = sum / spectrumSize;
        Vector3 targetScale = initialScale * (1f + averageAmplitude * scaleMultiplier);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
    }

    // Call this after the transition to update the baseline scale.
    public void UpdateInitialScale()
    {
        initialScale = transform.localScale;
    }
}
