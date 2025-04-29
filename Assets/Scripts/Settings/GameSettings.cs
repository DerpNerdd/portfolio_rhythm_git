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
    public static float    MasterVolume    { get; set; }
    public static float    MusicVolume     { get; set; }
    public static float    EffectsVolume   { get; set; }
    public static float    AudioOffset     { get; set; }
    public static KeyCode[] LaneKeys       { get; set; } = new KeyCode[4] { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
    public static float    ScrollSpeed     { get; set; }

    public static event Action KeybindsChanged;
    public static event Action SettingsUpdated;

    /// <summary>
    /// Invoke SettingsUpdated event.
    /// </summary>
    public static void NotifySettingsUpdated()
    {
        SettingsUpdated?.Invoke();
    }

    /// <summary>
    /// Invoke KeybindsChanged event.
    /// </summary>
    public static void NotifyKeybindsChanged()
    {
        KeybindsChanged?.Invoke();
    }
}