// Assets/Scripts/Home Scene/HorizontalAudioVisualizer.cs
using UnityEngine;
using UnityEngine.UI;

public class HorizontalAudioVisualizer : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource bgAudioSource;

    [Header("UI")]
    public RectTransform container;
    public Image         barPrefab;
    public int           barCount = 64;

    [Header("Behavior")]
    public float maxHeight   = 200f;
    public float smoothSpeed = 8f;
    public int   spectrumSize = 128;

    RectTransform[] bars;
    AudioClip       lastClip = null;

    void Start()
    {
        if (bgAudioSource == null)
        {
            Debug.LogError("HorizontalAudioVisualizer: No AudioSource.");
            enabled = false;
            return;
        }

        if (container == null)
            container = GetComponent<RectTransform>();

        bars = new RectTransform[barCount];
        float w = container.rect.width / barCount;
        for (int i = 0; i < barCount; i++)
        {
            var img = Instantiate(barPrefab, container);
            img.type           = Image.Type.Simple;
            img.preserveAspect = false;

            var rt = img.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0,0);
            rt.pivot     = new Vector2(0.5f, 0f);
            rt.sizeDelta = new Vector2(w, 1f);
            rt.anchoredPosition = new Vector2(i * w + w * 0.5f, 0f);
            bars[i] = rt;
        }
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

        float w = container.rect.width / barCount;
        for (int i = 0; i < barCount; i++)
        {
            int idx   = Mathf.Clamp(Mathf.FloorToInt((float)i / barCount * spec.Length), 0, spec.Length - 1);
            float val = spec[idx] * maxHeight;
            float cur = bars[i].sizeDelta.y;
            bars[i].sizeDelta = new Vector2(w, Mathf.Lerp(cur, val, Time.deltaTime * smoothSpeed));
        }
    }
}
