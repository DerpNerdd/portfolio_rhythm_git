// Assets/Scripts/Audio/StaticSpectrumProvider.cs
using UnityEngine;
using System;

public static class SpectrumProvider
{
    static AudioSource src;
    static float[] rawData, real, imag;
    public static float[] Spectrum { get; private set; }
    static int channels, sampleLen, fftSize;

    /// <summary>
    /// Must call in Start (or on clip‐change) with your AudioSource
    /// and desired FFT window size (will be rounded up to a power-of-two).
    /// </summary>
    public static void Initialize(AudioSource audioSource, int requestedSize)
    {
        // 1) Clip or source invalid → fall back to silent zeros
        if (audioSource == null || audioSource.clip == null || audioSource.clip.samples * audioSource.clip.channels == 0)
        {
            Debug.LogWarning("SpectrumProvider.Initialize: invalid or empty clip → silent spectrum");
            int silentSize = Mathf.NextPowerOfTwo(Mathf.Max(2, requestedSize)) >> 1;
            Spectrum = new float[silentSize];
            for (int i = 0; i < silentSize; i++) Spectrum[i] = 0f;
            src = null;
            return;
        }

        // 2) Round up to power-of-two
        int newFft = Mathf.NextPowerOfTwo(Mathf.Max(2, requestedSize));
        if (audioSource == src && newFft == fftSize)
            return;  // no changes

        // 3) (Re)initialize buffers
        src       = audioSource;
        fftSize   = newFft;
        channels  = src.clip.channels;
        sampleLen = Mathf.Max(1, src.clip.samples * channels);

        rawData   = new float[sampleLen];
        src.clip.GetData(rawData, 0);

        real      = new float[fftSize];
        imag      = new float[fftSize];
        Spectrum  = new float[fftSize >> 1];
    }

    /// <summary>
    /// Call every frame after Initialize. Always fills Spectrum[] with [0–1] magnitudes.
    /// </summary>
    public static void UpdateSpectrum()
    {
        if (src == null || Spectrum == null) return;

        int pos = src.timeSamples * channels;
        // copy and zero-pad
        for (int i = 0; i < fftSize; i++)
        {
            int idx = (pos + i) % sampleLen;
            real[i] = rawData[idx];
            imag[i] = 0f;
        }

        // in-place Cooley–Tukey FFT
        FFT(real, imag);

        // normalize into Spectrum[]
        int half = fftSize >> 1;
        const float norm = 0.9f;
        for (int i = 0; i < half; i++)
        {
            float m = Mathf.Sqrt(real[i] * real[i] + imag[i] * imag[i]);
            Spectrum[i] = (m / fftSize) * norm;
        }
    }

    // --- standard FFT & swap; unchanged from before ---
    static void FFT(float[] r, float[] i)
    {
        int n = r.Length;
        // bit-reverse
        for (int j = 1, k = 0; j < n; j++)
        {
            int bit = n >> 1;
            for (; k >= bit; bit >>= 1) k -= bit;
            k += bit;
            if (j < k)
            {
                Swap(r, j, k);
                Swap(i, j, k);
            }
        }
        // Danielson-Lanczos
        for (int len = 2; len <= n; len <<= 1)
        {
            float ang = -2f * Mathf.PI / len;
            float wLenR = Mathf.Cos(ang), wLenI = Mathf.Sin(ang);
            for (int start = 0; start < n; start += len)
            {
                float wR = 1f, wI = 0f;
                int halfLen = len >> 1;
                for (int j = 0; j < halfLen; j++)
                {
                    int even = start + j, odd = even + halfLen;
                    float er = r[even], ei = i[even];
                    float or_ = r[odd], oi = i[odd];
                    float tr = wR * or_ - wI * oi;
                    float ti = wR * oi + wI * or_;
                    r[even] = er + tr;  i[even] = ei + ti;
                    r[odd]  = er - tr;  i[odd]  = ei - ti;
                    float nwR = wR * wLenR - wI * wLenI;
                    wI = wR * wLenI + wI * wLenR;
                    wR = nwR;
                }
            }
        }
    }

    static void Swap(float[] a, int x, int y)
    {
        float t = a[x]; a[x] = a[y]; a[y] = t;
    }
}
