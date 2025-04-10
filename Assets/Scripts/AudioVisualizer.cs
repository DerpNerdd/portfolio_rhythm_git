using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Assign your AudioSource here")]
    public AudioSource audioSource;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;
    // Use a power-of-two sample buffer size. We choose 128.
    public int spectrumSize = 128;

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
    [Tooltip("Minimum Y-scale multiplier for bars (i.e. base size) when intensity is very low")]
    public float minBarScaleY = 1.0f;
    [Tooltip("Maximum Y-scale multiplier for bars (when intensity is high)")]
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
    private float[] spectrumData;         // Array for the 128 spectrum samples
    private float[] downSampledSpectrum;  // Array for our desired 120 samples

    void Start()
    {
        // Error checking
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

        // Allocate the array for spectrum data.
        spectrumData = new float[spectrumSize];
        downSampledSpectrum = new float[desiredBands];

        // Instantiate and arrange the bars in a circular pattern.
        bars = new GameObject[numberOfBars];
        float angleStep = 360f / numberOfBars;
        for (int i = 0; i < numberOfBars; i++)
        {
            float angleDeg = i * angleStep;
            float angleRad = angleDeg * Mathf.Deg2Rad;
            // Position each bar along the circle using the specified radius.
            Vector3 pos = new Vector3(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius, 0);
            bars[i] = Instantiate(barPrefab, transform);
            bars[i].transform.localPosition = pos;
            // Rotate the bar so that it faces outward (subtract 90° to account for pivot differences).
            bars[i].transform.localRotation = Quaternion.Euler(0, 0, angleDeg - 90f);
        }
    }

void Update()
{
    // Step 1: Get the raw spectrum data from the audio source.
    audioSource.GetSpectrumData(spectrumData, 0, fftWindow);

    // Step 2: Downsample the 128 values to our target length (desiredBands) using linear interpolation.
    for (int i = 0; i < desiredBands; i++)
    {
        // Map index in [0, desiredBands-1] to a float index in [0, spectrumSize-1]
        float pos = i * (spectrumSize - 1f) / (desiredBands - 1f);
        int lower = Mathf.FloorToInt(pos);
        int upper = Mathf.Clamp(lower + 1, 0, spectrumSize - 1);
        float t = pos - lower;
        downSampledSpectrum[i] = Mathf.Lerp(spectrumData[lower], spectrumData[upper], t);
    }

    // Step 3: Compute raw intensities for each bar using the downsampled spectrum.
    float[] rawIntensities = new float[numberOfBars];
    for (int i = 0; i < numberOfBars; i++)
    {
        int sampleIndex = Mathf.RoundToInt(i * (desiredBands - 1f) / (numberOfBars - 1f));
        rawIntensities[i] = downSampledSpectrum[sampleIndex] * heightMultiplier;
        // We intentionally are not clamping here so we let the dynamic value be computed.
    }

    // Step 4: Smooth the intensities by averaging each bar's intensity with its immediate neighbors.
    float[] smoothedIntensities = new float[numberOfBars];
    for (int i = 0; i < numberOfBars; i++)
    {
        float prev = rawIntensities[(i - 1 + numberOfBars) % numberOfBars];
        float curr = rawIntensities[i];
        float next = rawIntensities[(i + 1) % numberOfBars];
        smoothedIntensities[i] = (prev + curr + next) / 3f;
    }

    // Step 5: Compute the dynamic maximum, then clamp it so that it does not exceed a preset max intensity.
    float dynamicMax = 0f;
    for (int i = 0; i < numberOfBars; i++)
    {
        dynamicMax = Mathf.Max(dynamicMax, smoothedIntensities[i]);
    }
    // Avoid dividing by zero
    dynamicMax = Mathf.Max(dynamicMax, 0.1f);
    // Clamp the dynamic maximum to the public maxIntensity.
    float effectiveMax = Mathf.Clamp(dynamicMax, 0.1f, maxIntensity);

    // Step 6: Remap, smooth, and update each bar's scale.
    for (int i = 0; i < numberOfBars; i++)
    {
        // Normalize the intensity between 0.1 and the effective max.
        float normalizedIntensity = Mathf.InverseLerp(0.1f, effectiveMax, smoothedIntensities[i]);
        // Apply a power function to compress or expand the range; lower exponent compresses high values.
        normalizedIntensity = Mathf.Pow(normalizedIntensity, intensityExponent);
        // Lerp from the minimum to the maximum Y-scale multiplier.
        float targetScaleY = Mathf.Lerp(minBarScaleY, maxBarScaleY, normalizedIntensity);

        // Smoothly interpolate the bar’s current Y-scale toward the target scale.
        float currentScaleY = bars[i].transform.localScale.y;
        float newScaleY = Mathf.Lerp(currentScaleY, targetScaleY, Time.deltaTime * 10f);

        // Set the bar's scale—fix the X-scale and Z-scale.
        bars[i].transform.localScale = new Vector3(baseBarWidth, newScaleY, 1f);

        // (Optional) If your bar prefab has a child (for the visible mesh), adjust its position so the bottom remains fixed.
        if (bars[i].transform.childCount > 0)
        {
            Transform barMesh = bars[i].transform.GetChild(0);
            float offset = baseBarHeight * (newScaleY - 1f) / 2f;
            barMesh.localPosition = new Vector3(0, offset, 0);
        }
    }
}


}
