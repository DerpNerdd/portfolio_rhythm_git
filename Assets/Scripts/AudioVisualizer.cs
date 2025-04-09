using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;         // Assign your AudioSource via the Inspector
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;
    public int spectrumSize = 256;

    [Header("Visualizer Settings")]
    public int numberOfBars = 64;           // Number of bars arranged around the circle
    public float radius = 3f;               // Radius along which the bars are positioned
    public GameObject barPrefab;            // Assign your bar prefab via the Inspector
    public float heightMultiplier = 50f;    // Multiplier for how tall the bars get (raw value)
    public float baseBarHeight = 1.2f;        // The natural height of your bar prefab
    public float baseBarWidth = 0.5f;         // The fixed width for each bar

    // New values to remap the spectrum intensity so all bars remain within a set range:
    public float minBarScaleY = 1.0f;         // Minimum Y-scale (i.e. base size) when intensity is very low
    public float maxBarScaleY = 2.5f;         // Maximum Y-scale multiplier when intensity is high

    private GameObject[] bars;
    private float[] spectrumData;
    public float maxIntensity = 1.0f; // Maximum intensity value to avoid extreme scaling

    void Start()
    {
        // Allocate the array for spectrum data.
        spectrumData = new float[spectrumSize];

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
    // Get the spectrum data from the audio source
    audioSource.GetSpectrumData(spectrumData, 0, fftWindow);

    int samplesPerBar = spectrumSize / numberOfBars; // e.g., 256/64 = 4
    // Temporary array to hold each bar's raw intensity value
    float[] rawIntensities = new float[numberOfBars];

    // Calculate raw intensity for each bar using the average of its spectrum samples.
    for (int i = 0; i < numberOfBars; i++)
    {
        float sum = 0f;
        int startIndex = i * samplesPerBar;
        int endIndex = startIndex + samplesPerBar;
        for (int j = startIndex; j < endIndex; j++)
        {
            sum += spectrumData[j];
        }
        // Multiply by your heightMultiplier and clamp to avoid extremes.
        rawIntensities[i] = Mathf.Clamp(sum / samplesPerBar * heightMultiplier, 0.1f, maxIntensity);
    }

    // Now perform a moving average across the bars to smooth out intensity differences.
    // We'll average each bar's intensity with its previous and next bar (wrapping around at the ends)
    float[] smoothedIntensities = new float[numberOfBars];
    for (int i = 0; i < numberOfBars; i++)
    {
        float prev = rawIntensities[(i - 1 + numberOfBars) % numberOfBars];
        float curr = rawIntensities[i];
        float next = rawIntensities[(i + 1) % numberOfBars];
        smoothedIntensities[i] = (prev + curr + next) / 3f;
    }

    // Now update the scale of each bar based on the smoothed intensity.
    for (int i = 0; i < numberOfBars; i++)
    {
        // Calculate the desired new Y-scale.
        // Here, we assume that a raw intensity equal to baseBarHeight gives a scale of 1.
        float targetScaleY = smoothedIntensities[i] / baseBarHeight;
        
        // Smoothly lerp from the current y-scale to the target
        float currentScaleY = bars[i].transform.localScale.y;
        float newScaleY = Mathf.Lerp(currentScaleY, targetScaleY, Time.deltaTime * 10f);

        // Set the bar's scale, forcing the x-scale to be fixed (baseBarWidth) and leaving z unchanged.
        bars[i].transform.localScale = new Vector3(baseBarWidth, newScaleY, 1f);

        // If your bar prefab's visual mesh is a child (so the bottom remains fixed), adjust its position:
        if (bars[i].transform.childCount > 0)
        {
            Transform barMesh = bars[i].transform.GetChild(0);
            // Calculate offset needed to keep the bottom anchored.
            float offset = baseBarHeight * (newScaleY - 1f) / 2f;
            barMesh.localPosition = new Vector3(0, offset, 0);
        }
    }
}

}
