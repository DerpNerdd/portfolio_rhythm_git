// Assets/Scripts/Settings/GameSettings.cs
using System;
using UnityEngine;

/// <summary>
/// Holds global game settings and broadcasts changes.
/// </summary>
public static class GameSettings
{
    public static bool     Fullscreen      { get; set; }
    public static int      TargetFrameRate { get; set; }
    public static float    BackgroundDim   { get; set; }
    public static float    VisualOffset    { get; set; }

    /// <summary>
    /// Master volume (0 to 0.7). Defaults to mid-point (slider at 50 ⇒ 0.35).
    /// </summary>
    public static float    MasterVolume    { get; set; } = 0.7f * 0.5f;

    /// <summary>
    /// Music volume (0 to 1). Defaults to max (slider at 100).
    /// </summary>
    public static float    MusicVolume     { get; set; } = 1f;

    /// <summary>
    /// Effects volume (0 to 1). Defaults to max (slider at 100).
    /// </summary>
    public static float    EffectsVolume   { get; set; } = 1f;

    public static float    AudioOffset     { get; set; }
    public static KeyCode[] LaneKeys       { get; set; } = new KeyCode[4] { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };

    /// <summary>
    /// Default scroll speed.
    /// </summary>
    public static float    ScrollSpeed     { get; set; } = 1f;

    public static event Action KeybindsChanged;
    public static event Action SettingsUpdated;

    public static void NotifySettingsUpdated()    => SettingsUpdated?.Invoke();
    public static void NotifyKeybindsChanged()    => KeybindsChanged?.Invoke();
}
