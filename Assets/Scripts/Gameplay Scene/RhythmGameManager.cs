// Assets/Scripts/Gameplay Scene/RhythmGameManager.cs
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class ChartNote {
    public float time;       // when to hit
    public int   laneIndex;  // which lane 0…N-1
}

[System.Serializable]
public class ChartData {
    public List<ChartNote> notes;
}

public class RhythmGameManager : MonoBehaviour
{
    [Header("References")]
    public NoteSpawner         noteSpawner;   // handles instantiation
    public RectTransform       hitLine;       // target y pos

    [Header("Gameplay")]
    public float               approachTime = 2f;

    [Header("UI Elements")]
    public TextMeshProUGUI     scoreText;
    public TextMeshProUGUI     comboText;
    public TextMeshProUGUI     accuracyText;
    public TextMeshProUGUI     judgmentText;

    [Header("Audio")]
    public AudioSource         audioPlayer;

    [Header("Input")]
    public KeyCode[]           laneKeys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
    public float               hitWindow = 0.15f;

    private int score, combo, hits, attempts;

    void Start()
    {
        if (audioPlayer.clip != null)
            audioPlayer.Play();

        UpdateUI();
        LoadAndSpawnNotes();
    }

    void Update()
    {
        // per‐lane key detection
        for (int lane = 0; lane < laneKeys.Length; lane++)
        {
            if (Input.GetKeyDown(laneKeys[lane]))
                TryHit(lane);
        }
    }

    void UpdateUI()
    {
        scoreText.text    = score.ToString("N0");
        comboText.text    = combo > 0 ? $"{combo}x" : "";
        float acc         = attempts > 0 ? (hits / (float)attempts) * 100f : 100f;
        accuracyText.text = $"{acc:F1}%";
    }

    void LoadAndSpawnNotes()
    {
        // load chart JSON
        string path = SelectedChart.ChartPath;
        var ta = Resources.Load<TextAsset>(path);
        if (ta == null)
        {
            Debug.LogError($"Couldn't load chart at '{path}'");
            return;
        }
        var chart = JsonUtility.FromJson<ChartData>(ta.text);

        // compute Y positions
        float spawnY = noteSpawner.noteContainer.rect.height;
        float hitY   = hitLine.anchoredPosition.y;

        foreach (var n in chart.notes)
        {
            // pass laneIndex into spawner & controller
            var rt = noteSpawner.SpawnNote(n.laneIndex, spawnY);
            if (rt != null)
            {
                var nc = rt.GetComponent<NoteController>();
                if (nc != null)
                    nc.Init(n.laneIndex, n.time, approachTime, spawnY, hitY, this);
            }
        }
    }

    void TryHit(int lane)
    {
        var allNotes  = noteSpawner.noteContainer.GetComponentsInChildren<NoteController>();
        NoteController best = null;
        float bestDelta = float.MaxValue;
        float now = audioPlayer.time;

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
            // tiered judgment
            string judgment;
            int points;
            if (bestDelta <= hitWindow * 0.33f) { judgment = "Perfect"; points = 300; }
            else if (bestDelta <= hitWindow * 0.67f) { judgment = "Good"; points = 100; }
            else { judgment = "OK"; points = 50; }

            RegisterHit(judgment, points);
            best.handled = true;
            Destroy(best.gameObject);
        }
    }

    public void RegisterHit(string judgment, int points)
    {
        score   += points;
        combo   += 1;
        hits    += 1;
        attempts+= 1;
        judgmentText.text = judgment;
        UpdateUI();
    }

    public void RegisterMiss()
    {
        combo = 0;
        attempts += 1;
        judgmentText.text = "Miss";
        UpdateUI();
    }
}
