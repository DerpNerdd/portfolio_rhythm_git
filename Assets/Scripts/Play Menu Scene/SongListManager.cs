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

    [Header("Main Info Panel")]
    [Tooltip("Controller for the right‑hand info display")]
    public MainInfoController mainInfo;

    [Header("Raycasting (optional)")]
    public GraphicRaycaster raycaster;
    public EventSystem     eventSystem;

    private readonly List<SongData>           allSongs    = new List<SongData>();
    private readonly List<SongItemController> controllers = new List<SongItemController>();
    private string                            searchQuery = "";

    private readonly string[] groupOptions = { "None", "Easy", "Medium", "Hard", "Extreme" };

    void Awake()
    {
        if (raycaster    == null) raycaster    = FindObjectOfType<GraphicRaycaster>();
        if (eventSystem == null) eventSystem = EventSystem.current ?? FindObjectOfType<EventSystem>();
    }

    void Start()
    {
        LoadSongsFromFolders();
        sortDropdown.onValueChanged.AddListener(_ => UpdateList());
        groupDropdown.onValueChanged.AddListener(_ => UpdateList());
        UpdateList();
    }

    void OnDestroy()
    {
        sortDropdown.onValueChanged.RemoveAllListeners();
        groupDropdown.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// Called by SearchBarController to apply text filtering.
    /// </summary>
    public void FilterSongs(string query)
    {
        searchQuery = (query ?? "").Trim().ToLowerInvariant();
        UpdateList();
    }

    /// <summary>
    /// Populates allSongs from Resources/Songs/{id}/SongData.json
    /// </summary>
    private void LoadSongsFromFolders()
    {
        var indexText = Resources.Load<TextAsset>("Songs/songIndex");
        if (indexText == null)
        {
            Debug.LogError("[SongListManager] Missing songIndex.json");
            return;
        }
        var index = JsonUtility.FromJson<SongIndex>(indexText.text);
        foreach (var id in index.songIDs)
        {
            var ta = Resources.Load<TextAsset>($"Songs/{id}/SongData");
            if (ta == null) continue;
            var song = JsonUtility.FromJson<SongData>(ta.text);
            if (song == null || string.IsNullOrEmpty(song.songName)) continue;

            song.coverArt  = Resources.Load<Sprite>($"Songs/{id}/cover");
            song.audioClip = Resources.Load<AudioClip>($"Songs/{id}/audio");
            allSongs.Add(song);
        }
    }

    /// <summary>
    /// Applies search, group, and sort filters then rebuilds the UI.
    /// </summary>
    private void UpdateList()
    {
        IEnumerable<SongData> list = allSongs;

        // text search
        if (!string.IsNullOrEmpty(searchQuery))
        {
            list = list.Where(s =>
                s.songName.ToLowerInvariant().Contains(searchQuery) ||
                s.artist.ToLowerInvariant().Contains(searchQuery)
            );
        }

        // group filter
        int gIdx = groupDropdown.value;
        if (gIdx > 0)
        {
            string gf = groupOptions[gIdx];
            list = list.Where(s => s.beatmaps.Any(b => b.displayName == gf));
        }

        // sort
        switch (sortDropdown.value)
        {
            case 1: list = list.OrderBy(s => s.songName); break;
            case 2: list = list.OrderBy(s => s.artist);   break;
            case 3: list = list.OrderByDescending(
                            s => s.audioClip != null ? s.audioClip.length : 0f
                        ); break;
        }

        PopulateList(list.ToList(), gIdx > 0 ? groupOptions[gIdx] : null);
    }

    private void PopulateList(List<SongData> songs, string groupFilter)
    {
        // clear
        foreach (var c in controllers)
            Destroy(c.gameObject);
        controllers.Clear();

        // instantiate
        foreach (var s in songs)
        {
            var go   = Instantiate(songItemPrefab, contentParent);
            var ctrl = go.GetComponent<SongItemController>();
            ctrl.manager = this;
            controllers.Add(ctrl);
            ctrl.Initialize(s, groupFilter);
        }

        // rebuild & reset scroll
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
        if (raycaster == null || eventSystem == null) return;
        var pd   = new PointerEventData(eventSystem) { position = Input.mousePosition };
        var hits = new List<RaycastResult>();
        raycaster.Raycast(pd, hits);

        if (!hits.Any(h => h.gameObject.GetComponentInParent<SongItemController>() != null))
            controllers.ForEach(c => c.AnimateCollapse());
    }

    public void NotifyItemExpanded(SongItemController expanded)
    {
        foreach (var c in controllers)
            if (c != expanded) c.Collapse();
    }

    /// <summary>
    /// Called by SongItemController when a difficulty entry is clicked.
    /// </summary>
    public void SelectDifficulty(SongData song, BeatmapInfo bm)
    {
        mainInfo?.UpdateMainInfo(song, bm);
    }

    /// <summary>
    /// Exposes a centralized layout rebuild call for SongItemController.
    /// </summary>
    public void LayoutRebuild(RectTransform rt)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
}
