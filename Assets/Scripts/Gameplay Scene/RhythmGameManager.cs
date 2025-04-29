// Assets/Scripts/Gameplay Scene/RhythmGameManager.cs
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class ChartNote
{
    public float time;
    public int   laneIndex;
}

[System.Serializable]
public class ChartData
{
    public List<ChartNote> notes;
}

public class RhythmGameManager : MonoBehaviour
{
    [Header("References")]
    public NoteSpawner   noteSpawner;
    public RectTransform hitLine;

    [Header("Gameplay")]
    public float beatsOnScreen = 4f;

    [Header("Timing Windows (s)")]
    public float perfectWindow = 0.10f;
    public float greatWindow   = 0.25f;
    public float goodWindow    = 0.30f;

    [Header("Scoring")]
    [Tooltip("Total score awarded if every note is Perfect")] 
    public int   targetScore     = 1000000;
    [Tooltip("Great hits are this fraction of Perfect")]  
    [Range(0f,1f)] public float greatMultiplier = 0.7f;
    [Tooltip("Good hits are this fraction of Perfect")]   
    [Range(0f,1f)] public float goodMultiplier  = 0.3f;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI judgmentText;

    [Header("Audio")]
    public AudioSource audioPlayer;

    private int    score, combo, hits, attempts;
    private float  approachTime, timeOffset;

    private int    totalNotes;
    private float  perNoteScore;

    void Start()
    {
        SettingsPersistence.Load();

        if (noteSpawner == null || noteSpawner.lanesContainer == null)
        {
            Debug.LogError("RhythmGameManager: noteSpawner or lanesContainer not assigned.");
            enabled = false;
            return;
        }
        if (hitLine == null)
        {
            Debug.LogError("RhythmGameManager: hitLine not assigned.");
            enabled = false;
            return;
        }
        if (SelectedChart.Song == null || SelectedChart.Beatmap == null)
        {
            Debug.LogError("RhythmGameManager: SelectedChart.Song or Beatmap is null.");
            enabled = false;
            return;
        }

        ApplySettings();

        // Load chart
        string path = SelectedChart.ChartPath;
        var ta = Resources.Load<TextAsset>(path);
        ChartData chart = null;
        if (ta != null)
        {
            try { chart = JsonUtility.FromJson<ChartData>(ta.text); }
            catch { chart = null; }
        }
        if (chart == null || chart.notes == null || chart.notes.Count == 0)
        {
            var all = SMParser.ParseAll(ta);
            if (all != null && all.Count > 0)
            {
                all.TryGetValue(SelectedChart.Beatmap.displayName, out chart);
                if (chart == null) chart = new List<ChartData>(all.Values)[0];
            }
        }
        if (chart == null || chart.notes.Count == 0)
        {
            Debug.LogError("RhythmGameManager: No valid chart data.");
            return;
        }

        // Dynamic scoring calc
        totalNotes   = chart.notes.Count;
        perNoteScore = (float)targetScore / totalNotes;

        // Set volume
        audioPlayer.volume = GameSettings.MasterVolume * GameSettings.MusicVolume;

        // Spawn notes
        float spawnY = noteSpawner.lanesContainer.rect.height;
        float hitY   = hitLine.anchoredPosition.y;
        foreach (var n in chart.notes)
        {
            var rt = noteSpawner.SpawnNote(n.laneIndex, spawnY);
            rt?.GetComponent<NoteController>()?
               .Init(n.laneIndex, n.time + timeOffset, approachTime, spawnY, hitY, this);
        }

        // Play music
        if (SelectedChart.Song.audioClip != null)
            audioPlayer.clip = SelectedChart.Song.audioClip;
        if (audioPlayer.clip != null)
            audioPlayer.Play();

        UpdateUI();
    }

    void Update()
    {
        ApplySettings();
        var keys = GameSettings.LaneKeys;
        for (int lane = 0; lane < keys.Length; lane++)
            if (Input.GetKeyDown(keys[lane]))
                TryHit(lane);
    }

    void ApplySettings()
    {
        timeOffset = GameSettings.AudioOffset;
        float bpm      = SelectedChart.Beatmap?.bpm > 0f ? SelectedChart.Beatmap.bpm : 120f;
        float baseTime = beatsOnScreen * (60f / bpm);
        float speed    = GameSettings.ScrollSpeed > 0f ? GameSettings.ScrollSpeed : 1f;
        approachTime   = baseTime / speed;
    }

    void TryHit(int lane)
    {
        float now = audioPlayer.time;
        var notes = noteSpawner.lanesContainer.GetComponentsInChildren<NoteController>();
        NoteController best = null;
        float bestDelta = float.MaxValue;

        foreach (var nc in notes)
        {
            if (nc.laneIndex != lane || nc.handled) continue;
            float delta = Mathf.Abs(now - (nc.noteTime - timeOffset));
            if (delta < bestDelta)
            {
                bestDelta = delta;
                best = nc;
            }
        }

        if (best == null)
        {
            RegisterMiss();
            return;
        }

        // Miss if outside good window
        if (bestDelta > goodWindow)
        {
            best.handled = true;
            Destroy(best.gameObject);
            RegisterMiss();
            return;
        }

        // Score tiers
        string judgment;
        int points;
        if (bestDelta <= perfectWindow)
        {
            judgment = "Perfect";
            points   = Mathf.RoundToInt(perNoteScore);
        }
        else if (bestDelta <= greatWindow)
        {
            judgment = "Great";
            points   = Mathf.RoundToInt(perNoteScore * greatMultiplier);
        }
        else
        {
            judgment = "Good";
            points   = Mathf.RoundToInt(perNoteScore * goodMultiplier);
        }

        best.handled = true;
        Destroy(best.gameObject);
        RegisterHit(judgment, points);
    }

    void RegisterHit(string judgment, int points)
    {
        score    += points;
        combo    += points > 0 ? 1 : 0;
        hits     += points > 0 ? 1 : 0;
        attempts += 1;
        judgmentText.text = judgment;
        UpdateUI();
    }

    public void RegisterMiss()
    {
        combo    = 0;
        attempts += 1;
        judgmentText.text = "Miss";
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text    = score.ToString("N0");
        comboText.text    = combo > 0 ? $"{combo}x" : string.Empty;
        float acc         = attempts > 0 ? (hits / (float)attempts) * 100f : 100f;
        accuracyText.text = $"{acc:F1}%";
    }
}
