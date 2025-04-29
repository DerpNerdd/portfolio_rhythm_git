// Assets/Scripts/Gameplay Scene/HorizontalAudioVisualizer.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class HorizontalAudioVisualizer : MonoBehaviour
{
    [Header("Audio Source")]
    [Tooltip("The AudioSource playing your song")]
    public AudioSource audioSource;

    [Header("UI Setup")]
    [Tooltip("Parent RectTransform in your Canvas (behind notes)")]
    public RectTransform  container;
    [Tooltip("A UI Image prefab for a single bar; just set its Material")]
    public Image          barPrefab;
    [Tooltip("Number of bars to generate horizontally")]
    public int            barCount = 64;

    [Header("Bar Scaling")]
    [Tooltip("Max bar height in px")]
    public float          maxHeight = 200f;
    [Tooltip("How quickly bars interpolate")]
    public float          smoothSpeed = 8f;

    [Header("FFT Settings")]
    [Tooltip("Window type for FFT")]
    public FFTWindow      window = FFTWindow.BlackmanHarris;

    private float[]               spectrum;
    private RectTransform[]       bars;

    void Start()
    {
        // sanity
        if (audioSource == null) audioSource = FindObjectOfType<AudioSource>();
        if (container   == null) container   = GetComponent<RectTransform>();

        // allocate
        spectrum = new float[barCount];
        bars     = new RectTransform[barCount];

        // create bars
        float width = container.rect.width / barCount;
        for (int i = 0; i < barCount; i++)
        {
            var img = Instantiate(barPrefab, container);
            img.type = Image.Type.Simple;
            img.preserveAspect = false;

            var rt = img.rectTransform;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot     = new Vector2(0.5f, 0f);
            rt.sizeDelta = new Vector2(width, 1f);
            rt.anchoredPosition = new Vector2(i * width + width * 0.5f, 0);

            bars[i] = rt;
        }
    }

    void Update()
    {
        // grab spectrum
        audioSource.GetSpectrumData(spectrum, 0, window);

        // update each bar
        for (int i = 0; i < barCount; i++)
        {
            float magnitude = spectrum[i] * maxHeight * 10f; // boost factor
            float h = Mathf.Clamp(magnitude, 1f, maxHeight);
            // smooth
            float curH = bars[i].sizeDelta.y;
            float newH = Mathf.Lerp(curH, h, Time.deltaTime * smoothSpeed);

            var sd = bars[i].sizeDelta;
            sd.y = newH;
            bars[i].sizeDelta = sd;
        }
    }
}
