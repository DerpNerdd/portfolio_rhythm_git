// AudioVisualizer.cs
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Assign your AudioSource here. If left null or disabled, it will:\n1) Try MusicPlayer.instance\n2) Fall back to the GameObject tagged 'HomeAudio'")]
    public AudioSource audioSource;

    [Tooltip("Size of the spectrum data array. Typical: 128, 256, etc.")]
    public int spectrumSize = 128;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    [Header("Visualizer Settings")]
    public int numberOfBars = 64;
    public float radius = 3f;
    public GameObject barPrefab;
    public float heightMultiplier = 50f;
    public float baseBarHeight = 1.2f;
    public float baseBarWidth = 0.5f;

    [Header("Bar Sensitivity Mapping")]
    public float minBarScaleY = 1.0f;
    public float maxBarScaleY = 2.5f;
    public float maxIntensity = 1.0f;

    [Header("Downsampling Settings")]
    public int desiredBands = 120;

    [Header("Non-linear Remapping")]
    public float intensityExponent = 0.5f;

    private GameObject[] bars;
    private float[] spectrumData;
    private float[] downSampledSpectrum;

    void Start()
    {
        // 1) Try inspector assignment
        if (audioSource == null || !audioSource.enabled)
        {
            // 2) Try persistent MusicPlayer
            if (MusicPlayer.instance != null)
            {
                audioSource = MusicPlayer.instance.GetComponent<AudioSource>();
            }
            else
            {
                // 3) Fallback to Home-scene AudioSource tagged "HomeAudio"
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
            Debug.LogError($"AudioVisualizer: No AudioSource found on {gameObject.name}. Disabling visualizer.");
            enabled = false;
            return;
        }

        if (barPrefab == null)
        {
            Debug.LogError($"AudioVisualizer: Bar Prefab not assigned on {gameObject.name}.");
            enabled = false;
            return;
        }

        spectrumData = new float[spectrumSize];
        downSampledSpectrum = new float[desiredBands];

        // Instantiate bars in a circle
        bars = new GameObject[numberOfBars];
        float angleStep = 360f / numberOfBars;
        for (int i = 0; i < numberOfBars; i++)
        {
            float angleDeg = i * angleStep;
            float rad = angleDeg * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0);
            bars[i] = Instantiate(barPrefab, transform);
            bars[i].transform.localPosition = pos;
            bars[i].transform.localRotation = Quaternion.Euler(0, 0, angleDeg - 90f);
        }
    }

    void Update()
    {
        // Spectrum analysis
        audioSource.GetSpectrumData(spectrumData, 0, fftWindow);

        // Downsample
        for (int i = 0; i < desiredBands; i++)
        {
            float p = i * (spectrumSize - 1f) / (desiredBands - 1f);
            int lo = Mathf.FloorToInt(p), hi = Mathf.Clamp(lo + 1, 0, spectrumSize - 1);
            float t = p - lo;
            downSampledSpectrum[i] = Mathf.Lerp(spectrumData[lo], spectrumData[hi], t);
        }

        // Map to bar heights
        float[] raw = new float[numberOfBars];
        for (int i = 0; i < numberOfBars; i++)
        {
            int idx = Mathf.RoundToInt(i * (desiredBands - 1f) / (numberOfBars - 1f));
            raw[i] = downSampledSpectrum[idx] * heightMultiplier;
        }

        // Smooth
        float[] smooth = new float[numberOfBars];
        for (int i = 0; i < numberOfBars; i++)
        {
            float prev = raw[(i - 1 + numberOfBars) % numberOfBars];
            float next = raw[(i + 1) % numberOfBars];
            smooth[i] = (prev + raw[i] + next) / 3f;
        }

        // Find dynamic max
        float dynMax = 0.1f;
        foreach (var v in smooth) dynMax = Mathf.Max(dynMax, v);
        float effMax = Mathf.Clamp(dynMax, 0.1f, maxIntensity);

        // Apply scales
        for (int i = 0; i < numberOfBars; i++)
        {
            float norm = Mathf.InverseLerp(0.1f, effMax, smooth[i]);
            norm = Mathf.Pow(norm, intensityExponent);
            float targetY = Mathf.Lerp(minBarScaleY, maxBarScaleY, norm);
            float curY = bars[i].transform.localScale.y;
            float newY = Mathf.Lerp(curY, targetY, Time.deltaTime * 10f);
            bars[i].transform.localScale = new Vector3(baseBarWidth, newY, 1f);

            if (bars[i].transform.childCount > 0)
            {
                float offset = baseBarHeight * (newY - 1f) / 2f;
                bars[i].transform.GetChild(0).localPosition = new Vector3(0, offset, 0);
            }
        }
    }
}
