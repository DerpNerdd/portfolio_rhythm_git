// Assets/Scripts/Home Scene/BeatBounce.cs
using UnityEngine;

public class BeatBounce : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource bgAudioSource;

    [Header("Bounce")]
    public float scaleMultiplier = 5f;
    public float smoothSpeed     = 4f;
    public int   spectrumSize    = 128;

    Vector3 initialScale;
    AudioClip lastClip = null;

    void Start()
    {
        if (bgAudioSource == null)
        {
            Debug.LogError("BeatBounce: No AudioSource.");
            enabled = false;
            return;
        }
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (bgAudioSource.clip != lastClip
            && bgAudioSource.clip != null
            && bgAudioSource.clip.samples * bgAudioSource.clip.channels > 0)
        {
            SpectrumProvider.Initialize(bgAudioSource, spectrumSize * 2);
            lastClip = bgAudioSource.clip;
        }

        SpectrumProvider.UpdateSpectrum();
        var spec = SpectrumProvider.Spectrum;
        if (spec == null || spec.Length == 0) return;

        float sum = 0f;
        foreach (var v in spec) sum += v;
        float avg = sum / spec.Length;

        Vector3 target = initialScale * (1f + avg * scaleMultiplier);
        transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * smoothSpeed);
    }

    /// <summary>
    /// Call this after your click‐transition to reset the bounce baseline.
    /// </summary>
    public void UpdateInitialScale()
    {
        initialScale = transform.localScale;
    }
}
