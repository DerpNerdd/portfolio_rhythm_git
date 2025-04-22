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

    [Header("Raycasting (optional)")]
    public GraphicRaycaster raycaster;
    public EventSystem     eventSystem;

    // Full dataset
    private readonly List<SongData>           allSongs    = new List<SongData>();
    // Active controllers for collapse
    private readonly List<SongItemController> controllers = new List<SongItemController>();

    // Holds the current search query (lowercase), empty = no filter
    private string searchQuery = "";

    private readonly string[] groupOptions = { "None", "Easy", "Medium", "Hard", "Extreme" };

    void Awake()
    {
        if (raycaster    == null) raycaster    = FindObjectOfType<GraphicRaycaster>();
        if (eventSystem == null) eventSystem = EventSystem.current ?? FindObjectOfType<EventSystem>();
    }

    void Start()
    {
        LoadSongsFromFolders();

        // wire dropdown callbacks
        sortDropdown.onValueChanged.AddListener(_ => UpdateList());
        groupDropdown.onValueChanged.AddListener(_ => UpdateList());

        // initial build
        UpdateList();
    }

    void OnDestroy()
    {
        sortDropdown.onValueChanged.RemoveAllListeners();
        groupDropdown.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// Public method for the SearchBarController to call.
    /// </summary>
    public void FilterSongs(string query)
    {
        searchQuery = (query ?? "").Trim().ToLowerInvariant();
        UpdateList();
    }

    /// <summary>
    /// Reads each Songs/{folder}/SongData.json + optional assets.
    /// </summary>
    private void LoadSongsFromFolders()
    {
        var indexText = Resources.Load<TextAsset>("Songs/songIndex");
        if (indexText == null)
        {
            Debug.LogError("[SongListManager] Missing Resources/Songs/songIndex.json");
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
    /// Applies search, group, and sort, then rebuilds the list.
    /// </summary>
    private void UpdateList()
    {
        IEnumerable<SongData> list = allSongs;

        // 1) Search filter
        if (!string.IsNullOrEmpty(searchQuery))
        {
            list = list.Where(s =>
                s.songName.ToLowerInvariant().Contains(searchQuery) ||
                s.artist.ToLowerInvariant().Contains(searchQuery)
            );
        }

        // 2) Group filter
        int gIdx = groupDropdown.value;
        if (gIdx > 0)
        {
            string gf = groupOptions[gIdx];
            list = list.Where(s => s.beatmaps.Any(b => b.displayName == gf));
        }

        // 3) Sort
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
        // clear existing
        foreach (var c in controllers)
            Destroy(c.gameObject);
        controllers.Clear();

        // instantiate new
        foreach (var s in songs)
        {
            var go   = Instantiate(songItemPrefab, contentParent);
            var ctrl = go.GetComponent<SongItemController>();
            ctrl.manager = this;
            controllers.Add(ctrl);
            ctrl.Initialize(s, groupFilter);
        }

        // rebuild layout & reset scroll
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
}
