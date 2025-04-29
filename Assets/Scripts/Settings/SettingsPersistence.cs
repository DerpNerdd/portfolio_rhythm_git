// Assets/Scripts/Settings/SettingsPersistence.cs
using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Serializes and deserializes GameSettings to disk so they persist across sessions.
/// </summary>
[Serializable]
public class SettingsData
{
    public bool      Fullscreen;
    public int       TargetFrameRate;
    public float     BackgroundDim;
    public float     VisualOffset;
    public float     MasterVolume;
    public float     MusicVolume;
    public float     EffectsVolume;
    public float     AudioOffset;
    public KeyCode[] LaneKeys;
    public float     ScrollSpeed;
}

public static class SettingsPersistence
{
    private static string FileName => Path.Combine(Application.persistentDataPath, "gamesettings.json");

    /// <summary>
    /// Saves current GameSettings to a JSON file on disk.
    /// </summary>
    public static void Save()
    {
        var data = new SettingsData
        {
            Fullscreen       = GameSettings.Fullscreen,
            TargetFrameRate  = GameSettings.TargetFrameRate,
            BackgroundDim    = GameSettings.BackgroundDim,
            VisualOffset     = GameSettings.VisualOffset,
            MasterVolume     = GameSettings.MasterVolume,
            MusicVolume      = GameSettings.MusicVolume,
            EffectsVolume    = GameSettings.EffectsVolume,
            AudioOffset      = GameSettings.AudioOffset,
            LaneKeys         = GameSettings.LaneKeys,
            ScrollSpeed      = GameSettings.ScrollSpeed
        };
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FileName, json);
            Debug.Log($"Settings saved to {FileName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save settings: {e.Message}");
        }
    }

    /// <summary>
    /// Loads GameSettings from disk if the file exists.
    /// </summary>
    public static void Load()
    {
        if (!File.Exists(FileName))
        {
            Debug.Log("SettingsPersistence: no settings file found, using defaults.");
            return;
        }
        try
        {
            string json = File.ReadAllText(FileName);
            var data = JsonUtility.FromJson<SettingsData>(json);
            if (data != null)
            {
                GameSettings.Fullscreen      = data.Fullscreen;
                GameSettings.TargetFrameRate = data.TargetFrameRate;
                GameSettings.BackgroundDim   = data.BackgroundDim;
                GameSettings.VisualOffset    = data.VisualOffset;
                GameSettings.MasterVolume    = data.MasterVolume;
                GameSettings.MusicVolume     = data.MusicVolume;
                GameSettings.EffectsVolume   = data.EffectsVolume;
                GameSettings.AudioOffset     = data.AudioOffset;
                GameSettings.LaneKeys        = data.LaneKeys;
                GameSettings.ScrollSpeed     = data.ScrollSpeed;
                Debug.Log($"Settings loaded from {FileName}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load settings: {e.Message}");
        }
    }
}
