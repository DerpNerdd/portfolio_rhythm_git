// Assets/Scripts/Gameplay Scene/RhythmGameManager.cs
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class ChartNote {
    public float time;
    public int   laneIndex;
}

[System.Serializable]
public class ChartData {
    public List<ChartNote> notes;
}

public class RhythmGameManager : MonoBehaviour
{
    [Header("References")]
    public NoteSpawner     noteSpawner;
    public RectTransform   hitLine;

    [Header("Gameplay")]
    public float           approachTime   = 2f;
    [Tooltip("± seconds tolerance for PERFECT")]
    public float           hitWindow      = 0.15f;
    [Tooltip("± seconds tolerance for WONDERFUL (must be < hitWindow)")]
    public float           wonderfulWindow= 0.05f;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI judgmentText;

    [Header("Audio")]
    public AudioSource     audioPlayer;

    [Header("Input")]
    public KeyCode[]       laneKeys  = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };

    private int score, combo, hits, attempts;

    void Start()
    {
        // play the chart’s song
        if (SelectedChart.Song?.audioClip != null)
            audioPlayer.clip = SelectedChart.Song.audioClip;
        if (audioPlayer.clip != null)
            audioPlayer.Play();
        else
            Debug.LogWarning("No AudioClip on SelectedChart.Song!");

        UpdateUI();
        LoadAndSpawnNotes();
    }

    void Update()
    {
        for (int lane = 0; lane < laneKeys.Length; lane++)
            if (Input.GetKeyDown(laneKeys[lane]))
                TryHit(lane);
    }

    void UpdateUI()
    {
        scoreText.text    = score.ToString("N0");
        comboText.text    = combo > 0 ? $"{combo}x" : "";
        float acc         = attempts > 0 ? (hits/(float)attempts)*100f : 100f;
        accuracyText.text = $"{acc:F1}%";
    }

    void LoadAndSpawnNotes()
    {
        var ta = Resources.Load<TextAsset>(SelectedChart.ChartPath);
        if (ta == null) { Debug.LogError("Couldn't load chart!"); return; }
        var chart = JsonUtility.FromJson<ChartData>(ta.text);

        float spawnY = noteSpawner.lanesContainer.rect.height;
        float hitY   = hitLine.anchoredPosition.y;

        foreach (var n in chart.notes)
        {
            var rt = noteSpawner.SpawnNote(n.laneIndex, spawnY);
            if (rt == null) continue;
            var nc = rt.GetComponent<NoteController>();
            nc?.Init(n.laneIndex, n.time, approachTime, spawnY, hitY, this);
        }
    }

    void TryHit(int lane)
    {
        var allNotes = noteSpawner.lanesContainer.GetComponentsInChildren<NoteController>();
        NoteController best = null;
        float bestDelta = float.MaxValue;
        float now = audioPlayer.time;

        // find closest note in time
        foreach (var nc in allNotes)
        {
            if (nc.laneIndex != lane || nc.handled) continue;
            float delta = Mathf.Abs(now - nc.noteTime);
            if (delta < bestDelta)
            {
                bestDelta = delta;
                best = nc;
            }
        }

        if (best != null && bestDelta <= hitWindow)
        {
            // 1) Wonderful tier
            string judgment; int points;
            if (bestDelta <= wonderfulWindow)
            {
                judgment = "Wonderful"; 
                points   = 500;
            }
            // 2) Perfect
            else if (bestDelta <= hitWindow * 0.33f)
            {
                judgment = "Perfect"; 
                points   = 300;
            }
            // 3) Good
            else if (bestDelta <= hitWindow * 0.67f)
            {
                judgment = "Good";    
                points   = 100;
            }
            // 4) OK
            else
            {
                judgment = "OK";      
                points   =  50;
            }

            RegisterHit(judgment, points);
            best.handled = true;
            Destroy(best.gameObject);
        }
    }

    public void RegisterHit(string judgment, int points)
    {
        score    += points;
        combo    += 1;
        hits     += 1;
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
}
