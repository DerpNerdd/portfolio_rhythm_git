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
    [Tooltip("Content RectTransform under ScrollView/Viewport")]
    public RectTransform contentParent;
    [Tooltip("Prefab of a SongItem")]
    public GameObject songItemPrefab;
    [Tooltip("ScrollRect for the song list")]
    public ScrollRect scrollRect;

    [Header("Filters & Sorters")]
    [Tooltip("Dropdown for sort (None, Title, Artist, Length)")]
    public TMP_Dropdown sortDropdown;
    [Tooltip("Dropdown for group (None, Easy, Medium, Hard, Extreme)")]
    public TMP_Dropdown groupDropdown;

    [Header("Right-Side Panels")]
    [Tooltip("Controller for the main song info display")]
    public MainInfoController mainInfo;
    [Tooltip("Controller for the song statistics section")]
    public SongStatsController statsController;

    [Header("Specific Diff Stats")]
    [Tooltip("Controller for Section 1 (mapper/source/genre/etc.)")]
    public DiffSection1Controller section1Controller;

    [Tooltip("Controller for HP/Acc/Star bars")]
    public DiffSection2Controller section2Controller;

    [Header("Audio Preview")]
    [Tooltip("AudioSource for background preview music")]
    public AudioSource previewAudioSource;

    [Header("Optional Raycast Setup")]
    public EventSystem eventSystem;

    private readonly List<SongData> allSongs    = new List<SongData>();
    private readonly List<SongItemController> controllers = new List<SongItemController>();
    private string searchQuery = "";
    private SongData currentSong;
    private BeatmapInfo currentBeatmap;
    private readonly string[] groupOptions = { "None","Easy","Medium","Hard","Extreme" };

    void Awake()
    {
        if (eventSystem == null)
            eventSystem = EventSystem.current ?? FindObjectOfType<EventSystem>();

        if (previewAudioSource != null)
        {
            previewAudioSource.loop = true;
            previewAudioSource.playOnAwake = false;
            previewAudioSource.volume = 0.2f;
        }
    }

    void Start()
    {
        LoadSongsFromFolders();
        sortDropdown.onValueChanged.AddListener(_ => UpdateList());
        groupDropdown.onValueChanged.AddListener(_ => UpdateList());
        UpdateList();

        // Auto-select and expand the first song
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
        searchQuery = query?.Trim().ToLowerInvariant() ?? "";
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
            var ta = Resources.Load<TextAsset>($"Songs/{id}/SongData");
            if (ta == null) continue;
            var song = JsonUtility.FromJson<SongData>(ta.text);
            if (song == null || string.IsNullOrEmpty(song.songName)) continue;

            song.coverArt  = Resources.Load<Sprite>($"Songs/{id}/cover");
            song.mainCover = Resources.Load<Sprite>($"Songs/{id}/mainCover");
            song.audioClip = Resources.Load<AudioClip>($"Songs/{id}/song");

            allSongs.Add(song);
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
            var gf = groupOptions[g];
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
        foreach (var c in controllers)
            Destroy(c.gameObject);
        controllers.Clear();

        foreach (var s in songs)
        {
            var go  = Instantiate(songItemPrefab, contentParent);
            var ctl = go.GetComponent<SongItemController>();
            ctl.manager = this;
            controllers.Add(ctl);
            ctl.Initialize(s, groupFilter);
        }

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

        // collapse any open items but do NOT change the selected song
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

public void SelectDifficulty(SongData song, BeatmapInfo bm)
{
    // store selection
    currentSong      = song;
    currentBeatmap   = bm;

    // update UI panels
    mainInfo?.UpdateMainInfo(song, bm);
    statsController?.UpdateStats(song, bm);
    section1Controller?.UpdateSection(bm);
    section2Controller?.UpdateSection2(bm);


    if (previewAudioSource == null)
        return;

    if (song.audioClip != null)
    {
        // if it's a new clip OR we aren't currently playing, restart it
        if (previewAudioSource.clip != song.audioClip || !previewAudioSource.isPlaying)
        {
            previewAudioSource.clip = song.audioClip;
            previewAudioSource.time = song.audioClip.length * 0.5f;
            previewAudioSource.Play();
        }
    }
    else
    {
        // no audio: stop and clear the clip so later selections will trigger a restart
        previewAudioSource.Stop();
        previewAudioSource.clip = null;
    }
}


    public void LayoutRebuild(RectTransform rt)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
}
