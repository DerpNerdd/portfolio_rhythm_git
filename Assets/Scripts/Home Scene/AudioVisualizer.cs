using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Assign your AudioSource here (if left empty or disabled, it will auto-find the persistent MusicPlayer's AudioSource)")]
    public AudioSource audioSource;

    [Tooltip("Size of the spectrum data array. Typical values: 128, 256, etc.")]
    public int spectrumSize = 128;

    [Tooltip("FFT window type for spectrum analysis.")]
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    [Header("Visualizer Settings")]
    [Tooltip("Number of bars arranged around the circle")]
    public int numberOfBars = 64;

    [Tooltip("Radius along which the bars are positioned")]
    public float radius = 3f;

    [Tooltip("Assign your bar prefab here")]
    public GameObject barPrefab;

    [Tooltip("Multiplier for how tall the bars get (raw value)")]
    public float heightMultiplier = 50f;

    [Tooltip("The natural height (in world units) of your bar prefab")]
    public float baseBarHeight = 1.2f;

    [Tooltip("The fixed width for each bar")]
    public float baseBarWidth = 0.5f;

    [Header("Bar Sensitivity Mapping")]
    [Tooltip("Minimum Y-scale multiplier for bars when intensity is very low")]
    public float minBarScaleY = 1.0f;

    [Tooltip("Maximum Y-scale multiplier for bars when intensity is high")]
    public float maxBarScaleY = 2.5f;

    [Tooltip("Maximum intensity value (raw) to clamp extreme values")]
    public float maxIntensity = 1.0f;

    [Header("Downsampling Settings")]
    [Tooltip("Target number of spectrum bands for a 'feel' of 120")]
    public int desiredBands = 120;

    [Header("Non-linear Remapping")]
    [Tooltip("Exponent for non-linear remapping of intensity (e.g., 0.5 compresses high values)")]
    public float intensityExponent = 0.5f;

    private GameObject[] bars;
    private float[] spectrumData;         // Array for the spectrum samples
    private float[] downSampledSpectrum;  // Array for our downsampled bands

    void Start()
    {
        // If the audio source is null or disabled, try to get it from MusicPlayer.
        if (audioSource == null || !audioSource.enabled)
        {
            if (MusicPlayer.instance != null)
                audioSource = MusicPlayer.instance.GetComponent<AudioSource>();
            else
                Debug.LogError("AudioVisualizer: No persistent MusicPlayer found on " + gameObject.name);
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioVisualizer: AudioSource is not assigned in the Inspector!");
            return;
        }
        if (barPrefab == null)
        {
            Debug.LogError("AudioVisualizer: Bar Prefab is not assigned in the Inspector!");
            return;
        }

        spectrumData = new float[spectrumSize];
        downSampledSpectrum = new float[desiredBands];

        // Instantiate and arrange the bars in a circular pattern.
        bars = new GameObject[numberOfBars];
        float angleStep = 360f / numberOfBars;
        for (int i = 0; i < numberOfBars; i++)
        {
            float angleDeg = i * angleStep;
            float angleRad = angleDeg * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius, 0);
            bars[i] = Instantiate(barPrefab, transform);
            bars[i].transform.localPosition = pos;
            bars[i].transform.localRotation = Quaternion.Euler(0, 0, angleDeg - 90f);
        }
    }

    void Update()
    {
        // Get the raw spectrum data from the audio source.
        audioSource.GetSpectrumData(spectrumData, 0, fftWindow);

        // Downsample the spectrum data using linear interpolation.
        for (int i = 0; i < desiredBands; i++)
        {
            float pos = i * (spectrumSize - 1f) / (desiredBands - 1f);
            int lower = Mathf.FloorToInt(pos);
            int upper = Mathf.Clamp(lower + 1, 0, spectrumSize - 1);
            float t = pos - lower;
            downSampledSpectrum[i] = Mathf.Lerp(spectrumData[lower], spectrumData[upper], t);
        }

        // Compute raw intensities for each bar.
        float[] rawIntensities = new float[numberOfBars];
        for (int i = 0; i < numberOfBars; i++)
        {
            int sampleIndex = Mathf.RoundToInt(i * (desiredBands - 1f) / (numberOfBars - 1f));
            rawIntensities[i] = downSampledSpectrum[sampleIndex] * heightMultiplier;
        }

        // Smooth the intensities by averaging with neighbors.
        float[] smoothedIntensities = new float[numberOfBars];
        for (int i = 0; i < numberOfBars; i++)
        {
            float prev = rawIntensities[(i - 1 + numberOfBars) % numberOfBars];
            float curr = rawIntensities[i];
            float next = rawIntensities[(i + 1) % numberOfBars];
            smoothedIntensities[i] = (prev + curr + next) / 3f;
        }

        // Find dynamic maximum and clamp it.
        float dynamicMax = 0f;
        for (int i = 0; i < numberOfBars; i++)
            dynamicMax = Mathf.Max(dynamicMax, smoothedIntensities[i]);
        dynamicMax = Mathf.Max(dynamicMax, 0.1f);
        float effectiveMax = Mathf.Clamp(dynamicMax, 0.1f, maxIntensity);

        // Remap and smooth each bar's scale.
        for (int i = 0; i < numberOfBars; i++)
        {
            float normalizedIntensity = Mathf.InverseLerp(0.1f, effectiveMax, smoothedIntensities[i]);
            normalizedIntensity = Mathf.Pow(normalizedIntensity, intensityExponent);
            float targetScaleY = Mathf.Lerp(minBarScaleY, maxBarScaleY, normalizedIntensity);
            float currentScaleY = bars[i].transform.localScale.y;
            float newScaleY = Mathf.Lerp(currentScaleY, targetScaleY, Time.deltaTime * 10f);
            bars[i].transform.localScale = new Vector3(baseBarWidth, newScaleY, 1f);

            if (bars[i].transform.childCount > 0)
            {
                Transform barMesh = bars[i].transform.GetChild(0);
                float offset = baseBarHeight * (newScaleY - 1f) / 2f;
                barMesh.localPosition = new Vector3(0, offset, 0);
            }
        }
    }
}
