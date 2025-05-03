// BeatBounce.cs
using UnityEngine;

public class BeatBounce : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Assign your AudioSource here. If left null or disabled, it will:\n1) Try MusicPlayer.instance\n2) Fall back to the GameObject tagged 'HomeAudio'")]
    public AudioSource audioSource;
    public int spectrumSize = 128;

    [Header("Bounce Settings")]
    public float scaleMultiplier = 5f;
    public float smoothSpeed = 4f;

    private Vector3 initialScale;
    private float[] spectrumData;

    void Start()
    {
        // 1) Try inspector
        if (audioSource == null || !audioSource.enabled)
        {
            // 2) Try persistent MusicPlayer
            if (MusicPlayer.instance != null)
            {
                audioSource = MusicPlayer.instance.GetComponent<AudioSource>();
            }
            else
            {
                // 3) Fallback to HomeAudio
                var go = GameObject.FindWithTag("HomeAudio");
                if (go != null)
                {
                    var src = go.GetComponent<AudioSource>();
                    if (src != null)
                    {
                        if (!src.enabled) src.enabled = true;
                        audioSource = src;
                    }
                }
            }
        }

        if (audioSource == null)
        {
            Debug.LogError($"BeatBounce: No AudioSource found for {gameObject.name}. Disabling bounce.");
            enabled = false;
            return;
        }

        initialScale = transform.localScale;
        spectrumData = new float[spectrumSize];
    }

    void Update()
    {
        if (audioSource == null) return;

        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);
        float sum = 0f;
        foreach (var v in spectrumData) sum += v;
        float avg = sum / spectrumSize;

        Vector3 target = initialScale * (1f + avg * scaleMultiplier);
        transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * smoothSpeed);
    }

    // Call this if you need to re-baseline after a transition
    public void UpdateInitialScale()
    {
        initialScale = transform.localScale;
    }
}
