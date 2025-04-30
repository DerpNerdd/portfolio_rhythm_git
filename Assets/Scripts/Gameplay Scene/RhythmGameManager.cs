using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public NoteSpawner    noteSpawner;
    public RectTransform  hitLine;

    [Header("Group Transforms")]
    public RectTransform  gameplayContainer;
    public RectTransform  resultsSection;

    [Header("Result UI Texts")]
    public TextMeshProUGUI perfectCountText;
    public TextMeshProUGUI greatCountText;
    public TextMeshProUGUI goodCountText;
    public TextMeshProUGUI missCountText;
    public TextMeshProUGUI totalScoreText;

    [Header("Pause Menu")]
    [Tooltip("Root GameObject of your PauseMenu (initially disabled)")]
    public GameObject        pauseMenu;
    [Tooltip("Continue button inside PauseMenu")]
    public Button            continueButton;
    [Tooltip("Quit button inside PauseMenu")]
    public Button            quitButton;
    [Tooltip("A TextMeshProUGUI used for the countdown (e.g. “3…2…1…Go!”)")]
    public TextMeshProUGUI   countdownText;

    [Header("Animation Settings")]
    public float slideDuration      = 1f;
    public float gameplayTargetPosX = -3f;
    public float resultsTargetPosX  = -9.28f;

    [Header("Gameplay")]
    public float beatsOnScreen = 4f;

    [Header("Timing Windows (s)")]
    public float perfectWindow = 0.05f;
    public float greatWindow   = 0.15f;
    public float goodWindow    = 0.25f;

    [Header("Scoring")]
    public int   targetScore     = 1000000;
    [Range(0f,1f)] public float greatMultiplier = 0.7f;
    [Range(0f,1f)] public float goodMultiplier  = 0.3f;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI judgmentText;
    public LifeBarController lifeBar;

    [Header("Audio")]
    public AudioSource audioPlayer;

    // internal state
    int   score, combo, attempts;
    int   perfectCount, greatCount, goodCount, missCount;
    float approachTime, timeOffset, perNoteScore;
    int   totalNotes;
    bool  isResultsVisible = false;
    bool  isPaused         = false;

    void Start()
    {
        SettingsPersistence.Load();

        // --- sanity checks ---
        if (noteSpawner == null || noteSpawner.lanesContainer == null ||
            hitLine == null || gameplayContainer == null ||
            resultsSection == null || pauseMenu == null ||
            continueButton == null || quitButton == null ||
            countdownText == null ||
            SelectedChart.Song == null || SelectedChart.Beatmap == null)
        {
            Debug.LogError("RhythmGameManager: Missing references in Inspector.");
            enabled = false;
            return;
        }

        ApplySettings();

        // --- JSON parsing with fallback ---
        TextAsset ta = Resources.Load<TextAsset>(SelectedChart.ChartPath);
        ChartData chart = null;
        if (ta != null)
        {
            try
            {
                chart = JsonUtility.FromJson<ChartData>(ta.text);
                if (chart == null || chart.notes == null || chart.notes.Count == 0)
                    chart = null;
            }
            catch { chart = null; }
        }
        if (chart == null && ta != null)
        {
            var all = SMParser.ParseAll(ta);
            if (all != null && all.TryGetValue(SelectedChart.Beatmap.displayName, out var cd))
                chart = cd;
            else if (all != null && all.Count > 0)
                chart = new List<ChartData>(all.Values)[0];
        }
        if (chart == null)
        {
            Debug.LogError("RhythmGameManager: No valid chart data.");
            return;
        }

        // --- scoring setup ---
        totalNotes   = chart.notes.Count;
        perNoteScore = (float)targetScore / totalNotes;

        // --- audio setup ---
        audioPlayer.volume = GameSettings.MasterVolume * GameSettings.MusicVolume;
        audioPlayer.clip   = SelectedChart.Song.audioClip;
        audioPlayer.Play();

        // --- spawn notes ---
        float spawnY = noteSpawner.lanesContainer.rect.height;
        float hitY   = hitLine.anchoredPosition.y;
        foreach (var n in chart.notes)
        {
            var rt = noteSpawner.SpawnNote(n.laneIndex, spawnY);
            rt?.GetComponent<NoteController>()?
               .Init(n.laneIndex, n.time + timeOffset, approachTime, spawnY, hitY, this);
        }

        // --- results panel starts off-screen to right ---
        resultsSection.anchoredPosition = new Vector2(Screen.width + 10, resultsSection.anchoredPosition.y);

        // --- pause setup ---
        pauseMenu.SetActive(false);
        countdownText.gameObject.SetActive(false);
        // ensure buttons start non-interactable
        continueButton.interactable = false;
        quitButton.interactable     = false;
        // hook up button callbacks
        continueButton.onClick.AddListener(OnContinueButton);
        quitButton.onClick.AddListener(OnGoBackButton);

        UpdateUI();
    }

void Update()
{
    // toggle pause on Escape
    if (Input.GetKeyDown(KeyCode.Escape) && !isResultsVisible)
    {
        if (isPaused) ResumeWithCountdown();
        else         PauseGame();
    }

    // while paused or after results, skip gameplay
    if (isPaused || isResultsVisible) return;

    // handle note inputs
    ApplySettings();
    var keys = GameSettings.LaneKeys;
    for (int lane = 0; lane < keys.Length; lane++)
        if (Input.GetKeyDown(keys[lane]))
            TryHit(lane);

    // **only** show results when the track actually ends
    if (!isPaused && !audioPlayer.isPlaying && attempts > 0)
        ShowResults();
}


    void ApplySettings()
    {
        timeOffset = GameSettings.AudioOffset;
        float bpm      = SelectedChart.Beatmap.bpm > 0 ? SelectedChart.Beatmap.bpm : 120f;
        float baseTime = beatsOnScreen * (60f / bpm);
        float speed    = GameSettings.ScrollSpeed > 0f ? GameSettings.ScrollSpeed : 1f;
        approachTime   = baseTime / speed;
    }

    void TryHit(int lane)
    {
        float now = audioPlayer.time;
        var notes = noteSpawner.lanesContainer.GetComponentsInChildren<NoteController>();

        NoteController best     = null;
        float          bestDelta = float.MaxValue;

        foreach (var nc in notes)
        {
            if (nc.laneIndex != lane || nc.handled) continue;
            float delta = Mathf.Abs(now - (nc.noteTime - timeOffset));
            if (delta < bestDelta)
            {
                bestDelta = delta;
                best      = nc;
            }
        }

        if (best == null || bestDelta > goodWindow) return;

        string judgment; int points;
        if (bestDelta <= perfectWindow)
        {
            judgment = "Perfect";
            points   = Mathf.RoundToInt(perNoteScore);
            perfectCount++;
        }
        else if (bestDelta <= greatWindow)
        {
            judgment = "Great";
            points   = Mathf.RoundToInt(perNoteScore * greatMultiplier);
            greatCount++;
        }
        else
        {
            judgment = "Good";
            points   = Mathf.RoundToInt(perNoteScore * goodMultiplier);
            goodCount++;
        }

        best.handled = true;
        Destroy(best.gameObject);
        RegisterHit(judgment, points);
    }

    void RegisterHit(string judgment, int points)
    {
        score    += points;
        combo    += 1;
        attempts += 1;
        judgmentText.text = judgment;

        lifeBar.OnNoteHit();
        UpdateUI();
    }

    public void RegisterMiss()
    {
        combo    = 0;
        attempts += 1;
        missCount++;
        judgmentText.text = "Miss";

        lifeBar.OnNoteMiss();
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text    = score.ToString("N0");
        comboText.text    = combo > 1 ? $"{combo}x" : string.Empty;
        float acc = attempts > 0
            ? Mathf.Clamp((score / (perNoteScore * attempts)) * 100f, 0f, 100f)
            : 100f;
        accuracyText.text = $"{acc:F1}%";
    }

    /// <summary>
    /// Slide out gameplay & slide in results.
    /// </summary>
    public void ShowResults()
    {
        if (isResultsVisible) return;
        isResultsVisible = true;

        audioPlayer.Stop();

        perfectCountText.text = perfectCount.ToString();
        greatCountText.text   = greatCount.ToString();
        goodCountText.text    = goodCount.ToString();
        missCountText.text    = missCount.ToString();
        totalScoreText.text   = score.ToString("N0");

        StartCoroutine(AnimateResults());
    }

    IEnumerator AnimateResults()
    {
        Vector2 gameStart    = gameplayContainer.anchoredPosition;
        Vector2 gameEnd      = new Vector2(gameplayTargetPosX, gameStart.y);
        Vector2 resultsStart = resultsSection.anchoredPosition;
        Vector2 resultsEnd   = new Vector2(resultsTargetPosX, resultsStart.y);

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            gameplayContainer.anchoredPosition = Vector2.Lerp(gameStart,    gameEnd,    t);
            resultsSection.anchoredPosition   = Vector2.Lerp(resultsStart, resultsEnd, t);
            yield return null;
        }
        gameplayContainer.anchoredPosition = gameEnd;
        resultsSection.anchoredPosition   = resultsEnd;
    }

    // ─── Pause Logic ───────────────────────────────────────────────────────────

    void PauseGame()
    {
        isPaused = true;
        audioPlayer.Pause();

        // lock buttons until menu fully appears
        continueButton.interactable = false;
        quitButton.interactable     = false;

        pauseMenu.SetActive(true);
        StartCoroutine(EnablePauseButtonsNextFrame());
    }

    IEnumerator EnablePauseButtonsNextFrame()
    {
        yield return null;
        continueButton.interactable = true;
        quitButton.interactable     = true;
    }

    void ResumeWithCountdown()
    {
        // hide pause menu immediately
        pauseMenu.SetActive(false);
        // start the 3→1 countdown
        StartCoroutine(DoResumeCountdown());
    }

    IEnumerator DoResumeCountdown()
    {
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i >= 1; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "Go!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);

        isPaused = false;
        audioPlayer.Play();
    }

    /// <summary>
    /// Called by Retry button and Quit button alike.
    /// </summary>
    public void OnRetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnGoBackButton()
    {
        SceneManager.LoadScene("Play Menu Scene");
    }

    public void OnContinueButton()
    {
        ResumeWithCountdown();
    }
}
