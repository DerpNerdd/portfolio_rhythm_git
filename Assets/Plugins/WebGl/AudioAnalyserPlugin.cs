using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class AudioAnalyserPlugin
{
    // On WebGL builds (and not in the Editor), these map to our .jslib functions.
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void AudioAnalyser_Init();

    [DllImport("__Internal")]
    private static extern void AudioAnalyser_GetData(IntPtr dataPtr, int length);
    #else
    // Stub implementations for Editor or other platforms
    private static void AudioAnalyser_Init() { }
    private static void AudioAnalyser_GetData(IntPtr dataPtr, int length) { }
    #endif

    /// <summary>
    /// Call this once (e.g. in Start()) after you Begin Playback, to hook up the browser AnalyserNode.
    /// </summary>
    public static void Init()
    {
        AudioAnalyser_Init();
    }

    /// <summary>
    /// Fills the provided float[] with the current frequency data.
    /// On WebGL it uses the JS plugin; elsewhere it just clears the array.
    /// </summary>
    public static void GetSpectrumData(float[] buffer)
    {
        if (buffer == null || buffer.Length == 0)
            return;

        #if UNITY_WEBGL && !UNITY_EDITOR
        // Pin the managed array and pass the pointer to the plugin
        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        AudioAnalyser_GetData(handle.AddrOfPinnedObject(), buffer.Length);
        handle.Free();
        #else
        // In Editor or non-WebGL, just zero it so you don’t get garbage values
        Array.Clear(buffer, 0, buffer.Length);
        #endif
    }
}
