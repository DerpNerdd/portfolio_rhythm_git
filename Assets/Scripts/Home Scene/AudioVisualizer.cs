// Assets/Scripts/Home Scene/AudioVisualizer.cs
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioVisualizer : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public int         spectrumSize = 128;

    [Header("Visualizer Settings")]
    public int      numberOfBars      = 64;
    public float    radius            = 3f;
    public GameObject barPrefab;
    public float    heightMultiplier  = 50f;
    public float    baseBarHeight     = 1.2f;
    public float    baseBarWidth      = 0.5f;

    [Header("Mapping Settings")]
    public float    minBarScaleY      = 1.0f;
    public float    maxBarScaleY      = 2.5f;
    public float    maxIntensity      = 1.0f;
    public float    intensityExponent = 0.5f;

    [Header("Downsampling")]
    [Min(2)]
    public int desiredBands = 120;

    GameObject[] bars;
    float[]      downSampled;
    AudioClip    lastClip = null;

    void OnValidate()
    {
        spectrumSize   = Mathf.Max(1, spectrumSize);
        numberOfBars   = Mathf.Max(1, numberOfBars);
        desiredBands   = Mathf.Max(2, desiredBands);
        minBarScaleY   = Mathf.Max(0f, minBarScaleY);
        maxBarScaleY   = Mathf.Max(minBarScaleY, maxBarScaleY);
        baseBarHeight  = Mathf.Max(0f, baseBarHeight);
        baseBarWidth   = Mathf.Max(0f, baseBarWidth);
    }

    void Start()
    {
        // find/fallback AudioSource
        if (audioSource == null || !audioSource.enabled)
        {
            if (MusicPlayer.instance != null)
                audioSource = MusicPlayer.instance.GetComponent<AudioSource>();
            else
            {
                var go = GameObject.FindWithTag("HomeAudio");
                if (go != null) audioSource = go.GetComponent<AudioSource>();
            }
        }
        if (audioSource == null || !audioSource.enabled)
        {
            Debug.LogError("AudioVisualizer: No valid AudioSource.");
            enabled = false;
            return;
        }

        // Prepare our down-sample buffer
        downSampled = new float[desiredBands];

        // Instantiate bars around a circle
        bars = new GameObject[numberOfBars];
        float step = 360f / numberOfBars;
        for (int i = 0; i < numberOfBars; i++)
        {
            var b = Instantiate(barPrefab, transform);
            float a = (i * step) * Mathf.Deg2Rad;
            b.transform.localPosition = new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0f);
            b.transform.localRotation = Quaternion.Euler(0, 0, i * step - 90f);
            b.transform.localScale    = new Vector3(baseBarWidth, baseBarHeight, 1f);
            bars[i] = b;
        }
    }

    void Update()
    {
        // ONLY initialize when we actually have a clip loaded with samples
        if (audioSource.clip != lastClip
            && audioSource.clip != null
            && audioSource.clip.samples * audioSource.clip.channels > 0)
        {
            SpectrumProvider.Initialize(audioSource, spectrumSize * 2);
            lastClip = audioSource.clip;
        }

        // Always update—SpectrumProvider now has a valid clip or simply returns no-op
        SpectrumProvider.UpdateSpectrum();
        var spec = SpectrumProvider.Spectrum;
        if (spec == null || spec.Length == 0) return;

        // Downsample
        for (int i = 0; i < desiredBands; i++)
        {
            float p = i * (spec.Length - 1f) / (desiredBands - 1f);
            int lo = Mathf.FloorToInt(p);
            int hi = Mathf.Min(lo + 1, spec.Length - 1);
            downSampled[i] = Mathf.Lerp(spec[lo], spec[hi], p - lo);
        }

        // Map to bars
        for (int i = 0; i < numberOfBars; i++)
        {
            int idx = Mathf.RoundToInt(i * (desiredBands - 1f) / (numberOfBars - 1f));
            float raw = downSampled[idx] * heightMultiplier;

            // neighbor smoothing
            float prev   = downSampled[(idx - 1 + desiredBands) % desiredBands];
            float next   = downSampled[(idx + 1) % desiredBands];
            float smooth = (prev + raw + next) / 3f;

            float dynMax = Mathf.Max(0.1f, smooth);
            float norm   = Mathf.InverseLerp(0.1f, Mathf.Min(dynMax, maxIntensity), smooth);
            norm = Mathf.Pow(norm, intensityExponent);
            float targetY = Mathf.Lerp(minBarScaleY, maxBarScaleY, norm);

            var tr = bars[i].transform;
            tr.localScale = Vector3.Lerp(tr.localScale,
                new Vector3(baseBarWidth, targetY, 1f),
                Time.deltaTime * 10f);
        }
    }
}
