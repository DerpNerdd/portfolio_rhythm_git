// Assets/Scripts/Play Menu Scene/SongListManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SongListManager : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform      contentParent;
    public GameObject         songItemPrefab;
    public ScrollRect         scrollRect;

    [Header("Filters & Sorters")]
    public TMP_Dropdown       sortDropdown;
    public TMP_Dropdown       groupDropdown;

    [Header("Right-Side Panels")]
    public MainInfoController     mainInfo;
    public SongStatsController    statsController;
    public DiffSection1Controller section1Controller;
    public DiffSection2Controller section2Controller;
    public DiffSection3Controller section3Controller;

    [Header("Play Counts")]
    public SongPlayStatsController playStatsController;
    public LeaderboardsController   leaderboardsController;

    [Header("Audio Preview")]
    public AudioSource        previewAudioSource;

    [Header("Optional Raycast Setup")]
    public EventSystem        eventSystem;

    // ─── Internal Data ──────────────────────────────────────────────────────
    private readonly List<SongData>           allSongs    = new List<SongData>();
    private readonly List<SongItemController> controllers = new List<SongItemController>();
    private string                            searchQuery = "";
    private SongData                          currentSong;
    private BeatmapInfo                       currentBeatmap;
    private readonly string[]                 groupOptions = { "None","Easy","Medium","Hard","Extreme" };

    void Awake()
    {
        if (eventSystem == null)
            eventSystem = EventSystem.current ?? FindObjectOfType<EventSystem>();

        if (previewAudioSource != null)
        {
            previewAudioSource.loop      = true;
            previewAudioSource.playOnAwake= false;
            previewAudioSource.volume    = 0.2f;
        }
    }

    void Start()
    {
        LoadSongsFromFolders();
        sortDropdown.onValueChanged.AddListener(_ => UpdateList());
        groupDropdown.onValueChanged.AddListener(_ => UpdateList());
        UpdateList();

        // Auto-expand first entry
        if (controllers.Count > 0)
            controllers[0].mainButton.onClick.Invoke();
    }

    void OnDestroy()
    {
        sortDropdown.onValueChanged.RemoveAllListeners();
        groupDropdown.onValueChanged.RemoveAllListeners();
    }

    public void FilterSongs(string query)
    {
        searchQuery = (query ?? "").Trim().ToLowerInvariant();
        UpdateList();
    }

    private void LoadSongsFromFolders()
    {
        var idx = Resources.Load<TextAsset>("Songs/songIndex");
        if (idx == null)
        {
            Debug.LogError("Missing Songs/songIndex.json");
            return;
        }
        var index = JsonUtility.FromJson<SongIndex>(idx.text);

        foreach (var id in index.songIDs)
        {
            try
            {
                var ta = Resources.Load<TextAsset>($"Songs/{id}/SongData");
                if (ta == null)
                {
                    Debug.LogWarning($"[SongListManager] No SongData.json for '{id}', skipping");
                    continue;
                }

                // remap JSON keys (300→_300 etc)
                string json = ta.text
                    .Replace("\"300\"", "\"_300\"")
                    .Replace("\"200\"", "\"_200\"")
                    .Replace("\"100\"", "\"_100\"")
                    .Replace("\"50\"",  "\"_50\"");

                var song = JsonUtility.FromJson<SongData>(json);
                song.resourceFolderID = id;
                song.coverArt   = Resources.Load<Sprite>($"Songs/{id}/cover");
                song.mainCover  = Resources.Load<Sprite>($"Songs/{id}/mainCover");
                song.audioClip  = Resources.Load<AudioClip>($"Songs/{id}/song");

                allSongs.Add(song);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[SongListManager] Error loading '{id}': {ex.Message}");
            }
        }
    }

    private void UpdateList()
    {
        IEnumerable<SongData> list = allSongs;
        if (!string.IsNullOrEmpty(searchQuery))
            list = list.Where(s =>
                s.songName.ToLowerInvariant().Contains(searchQuery) ||
                s.artist.ToLowerInvariant().Contains(searchQuery)
            );

        int g = groupDropdown.value;
        if (g > 0)
        {
            string gf = groupOptions[g];
            list = list.Where(s => s.beatmaps.Any(b => b.displayName == gf));
        }

        switch (sortDropdown.value)
        {
            case 1: list = list.OrderBy(s => s.songName); break;
            case 2: list = list.OrderBy(s => s.artist);   break;
            case 3: list = list.OrderByDescending(
                            s => s.audioClip != null ? s.audioClip.length : 0f
                        ); break;
        }

        PopulateList(list.ToList(), g > 0 ? groupOptions[g] : null);
    }

    private void PopulateList(List<SongData> songs, string groupFilter)
    {
        // Destroy old
        foreach (var c in controllers) Destroy(c.gameObject);
        controllers.Clear();

        // Instantiate new
        foreach (var s in songs)
        {
            var go  = Instantiate(songItemPrefab, contentParent);
            var ctl = go.GetComponent<SongItemController>();
            ctl.manager = this;
            controllers.Add(ctl);
            ctl.Initialize(s, groupFilter);
        }

        // Layout fix
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent);
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            var ap = scrollRect.content.anchoredPosition;
            scrollRect.content.anchoredPosition = new Vector2(ap.x, 0f);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CheckClickOutside();
    }

    private void CheckClickOutside()
    {
        if (eventSystem == null) return;
        var pd   = new PointerEventData(eventSystem) { position = Input.mousePosition };
        var hits = new List<RaycastResult>();
        eventSystem.RaycastAll(pd, hits);

        // collapse but don’t change selection
        if (!hits.Any(r => r.gameObject.GetComponentInParent<SongItemController>() != null))
        {
            foreach (var c in controllers)
                c.Collapse();
        }
    }

    public void NotifyItemExpanded(SongItemController expanded)
    {
        foreach (var c in controllers)
            if (c != expanded) c.Collapse();
    }

    /// <summary>
    /// Called by UI or RandomButton: collapse current entry, pick a random song+beatmap,
    /// expand it, and select it as if clicked.
    /// </summary>
    public void PlayRandomSong()
    {
        // 1) Collapse open entries
        foreach (var c in controllers) c.Collapse();

        // 2) Pick random song & beatmap
        if (allSongs.Count == 0) return;
        var song = allSongs[Random.Range(0, allSongs.Count)];
        if (song.beatmaps == null || song.beatmaps.Count == 0) return;
        var bm   = song.beatmaps[Random.Range(0, song.beatmaps.Count)];

        // 3) Find its controller and expand it
        var ctl = controllers.FirstOrDefault(c => c.SongData == song);
        if (ctl != null) ctl.AnimateExpand();

        // 4) Drive UI exactly as if clicked
        SelectDifficulty(song, bm);
    }

    // <-- existing API used by SongItemController
    public void SelectDifficulty(SongData song, BeatmapInfo bm)
    {
        currentSong    = song;
        currentBeatmap = bm;
        SelectedChart.Song     = song;
        SelectedChart.Beatmap  = bm;
        SelectedChart.ChartPath= $"Songs/{song.resourceFolderID}/{bm.chartFile}";

        mainInfo?.UpdateMainInfo(song, bm);
        statsController?.UpdateStats(song, bm);
        section1Controller?.UpdateSection(bm);
        section2Controller?.UpdateSection2(bm);
        section3Controller?.UpdateSection3(bm);
        playStatsController?.UpdatePlayStats(bm);
        leaderboardsController?.UpdateLeaderboards(bm);

        if (previewAudioSource != null)
        {
            if (song.audioClip != null &&
                (previewAudioSource.clip != song.audioClip || !previewAudioSource.isPlaying))
            {
                previewAudioSource.clip = song.audioClip;
                previewAudioSource.time = song.audioClip.length * 0.5f;
                previewAudioSource.Play();
            }
            else if (song.audioClip == null)
            {
                previewAudioSource.Stop();
                previewAudioSource.clip = null;
            }
        }
    }

    public void LayoutRebuild(RectTransform rt)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
}
