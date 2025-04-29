// Assets/Scripts/Settings/SettingsManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Drives the in-game Settings UI and persists changes to disk.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("Visual Settings")]
    public Toggle       fullscreenToggle;
    public TMP_Dropdown fpsDropdown;
    public Slider       backgroundDimSlider;
    public Slider       visOffsetSlider;

    [Header("Audio Settings")]
    public Slider       mastersVolumeSlider;   // range 0–100
    public Slider       musicVolumeSlider;     // range 0–100
    public Slider       effectsVolumeSlider;   // range 0–100
    public Slider       audioOffsetSlider;

    [Header("Input Settings")]
    public Button[]     keybindButtons;  // 4 buttons
    public TMP_Text[]   keybindLabels;   // 4 labels
    public Slider       scrollSpeedSlider;

    int rebindingIndex = -1;

    void Awake()
    {
        // Load previously saved settings before UI init
        SettingsPersistence.Load();

        // Auto-save whenever settings or keybinds change
        GameSettings.SettingsUpdated   += SettingsPersistence.Save;
        GameSettings.KeybindsChanged   += SettingsPersistence.Save;

        // Apply fullscreen & target frame rate immediately
        Screen.fullScreen = GameSettings.Fullscreen;
        Application.targetFrameRate = GameSettings.TargetFrameRate;

        // Apply master volume clamp (0–0.7) initially to global listener
        AudioListener.volume = GameSettings.MasterVolume;
    }

    void Start()
    {
        // Visual UI initialization
        fullscreenToggle.isOn = GameSettings.Fullscreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);

        var fpsOptions = new List<string>{"30","60","90","120","165","240","Unlimited"};
        fpsDropdown.ClearOptions();
        fpsDropdown.AddOptions(fpsOptions);
        fpsDropdown.value = fpsOptions.IndexOf(GameSettings.TargetFrameRate > 0
            ? GameSettings.TargetFrameRate.ToString() : "Unlimited");
        fpsDropdown.onValueChanged.AddListener(OnFPSDropdown);

        backgroundDimSlider.value = GameSettings.BackgroundDim;
        backgroundDimSlider.onValueChanged.AddListener(OnBackgroundDimChanged);
        visOffsetSlider.value     = GameSettings.VisualOffset;
        visOffsetSlider.onValueChanged.AddListener(OnVisualOffsetChanged);

        // Audio UI initialization (slider values 0–100)
        mastersVolumeSlider.value = (GameSettings.MasterVolume / 0.7f) * 100f;
        musicVolumeSlider.value   = GameSettings.MusicVolume * 100f;
        effectsVolumeSlider.value = GameSettings.EffectsVolume * 100f;
        audioOffsetSlider.value   = GameSettings.AudioOffset;

        mastersVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        effectsVolumeSlider.onValueChanged.AddListener(OnEffectsVolumeChanged);
        audioOffsetSlider.onValueChanged.AddListener(OnAudioOffsetChanged);

        // Input keybind UI
        for (int i = 0; i < keybindButtons.Length; i++)
        {
            int idx = i;
            keybindLabels[idx].text = GameSettings.LaneKeys[idx].ToString();
            keybindButtons[idx].onClick.AddListener(() => StartRebind(idx));
        }

        scrollSpeedSlider.value = GameSettings.ScrollSpeed;
        scrollSpeedSlider.onValueChanged.AddListener(OnScrollSpeedChanged);
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid leaks
        GameSettings.SettingsUpdated   -= SettingsPersistence.Save;
        GameSettings.KeybindsChanged   -= SettingsPersistence.Save;
    }

    void Update()
    {
        if (rebindingIndex >= 0)
        {
            foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kc))
                {
                    GameSettings.LaneKeys[rebindingIndex] = kc;
                    keybindLabels[rebindingIndex].text   = kc.ToString();
                    GameSettings.NotifyKeybindsChanged();
                    rebindingIndex = -1;
                    break;
                }
            }
        }
    }

    void OnFullscreenToggle(bool on)
    {
        Screen.fullScreen = on;
        GameSettings.Fullscreen = on;
        GameSettings.NotifySettingsUpdated();
    }

    void OnFPSDropdown(int idx)
    {
        string val = fpsDropdown.options[idx].text;
        GameSettings.TargetFrameRate = (val == "Unlimited") ? -1 : int.Parse(val);
        Application.targetFrameRate   = GameSettings.TargetFrameRate;
        GameSettings.NotifySettingsUpdated();
    }

    void OnBackgroundDimChanged(float v)
    {
        GameSettings.BackgroundDim = v;
        GameSettings.NotifySettingsUpdated();
    }

    void OnVisualOffsetChanged(float v)
    {
        GameSettings.VisualOffset = v;
        GameSettings.NotifySettingsUpdated();
    }

    void OnMasterVolumeChanged(float v)
    {
        // slider 0–100 → 0–0.7 master
        float norm = Mathf.Clamp01(v / 100f);
        GameSettings.MasterVolume = norm * 0.7f;
        AudioListener.volume      = GameSettings.MasterVolume;
        GameSettings.NotifySettingsUpdated();
    }

    void OnMusicVolumeChanged(float v)
    {
        // slider 0–100 → 0–1 music
        GameSettings.MusicVolume = Mathf.Clamp01(v / 100f);
        GameSettings.NotifySettingsUpdated();
    }

    void OnEffectsVolumeChanged(float v)
    {
        // slider 0–100 → 0–1 effects
        GameSettings.EffectsVolume = Mathf.Clamp01(v / 100f);
        GameSettings.NotifySettingsUpdated();
    }

    void OnAudioOffsetChanged(float v)
    {
        GameSettings.AudioOffset = v;
        GameSettings.NotifySettingsUpdated();
    }

    void OnScrollSpeedChanged(float v)
    {
        GameSettings.ScrollSpeed = v;
        GameSettings.NotifySettingsUpdated();
    }

    void StartRebind(int idx)
    {
        rebindingIndex = idx;
        keybindLabels[idx].text = "...";
    }
}
