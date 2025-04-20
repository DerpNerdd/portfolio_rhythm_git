// Assets/Scripts/Play Menu Scene/SongListManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SongListManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag in the Content RectTransform under ScrollView/Viewport")]
    public RectTransform contentParent;

    [Tooltip("Drag in your SongItem prefab here")]
    public GameObject songItemPrefab;

    [Tooltip("Drag your Scroll View's ScrollRect component here")]
    public ScrollRect scrollRect;

    [Header("Raycasting (optional)")]
    public GraphicRaycaster raycaster;
    public EventSystem     eventSystem;

    private readonly List<SongItemController> controllers = new();
    private readonly List<SongData>           allSongs    = new();

    void Awake()
    {
        if (raycaster == null)
        {
            var canvas = GetComponentInParent<Canvas>();
            raycaster = canvas
                ? canvas.GetComponent<GraphicRaycaster>()
                : FindObjectOfType<GraphicRaycaster>();
        }
        if (eventSystem == null)
            eventSystem = EventSystem.current ?? FindObjectOfType<EventSystem>();
    }

    void Start()
    {
        LoadSongsFromFolders();
        PopulateList(allSongs);
    }

    void LoadSongsFromFolders()
    {
        var indexText = Resources.Load<TextAsset>("Songs/songIndex");
        if (indexText == null) return;
        var index = JsonUtility.FromJson<SongIndex>(indexText.text);

        foreach (var id in index.songIDs)
        {
            var dataText = Resources.Load<TextAsset>($"Songs/{id}/SongData");
            if (dataText == null) continue;

            var song = JsonUtility.FromJson<SongData>(dataText.text);
            if (song == null || string.IsNullOrEmpty(song.songName)) continue;

            song.coverArt  = Resources.Load<Sprite>($"Songs/{id}/cover");
            song.audioClip = Resources.Load<AudioClip>($"Songs/{id}/audio");

            allSongs.Add(song);
        }
    }

    /// <summary>
    /// Rebuilds the UI list from the provided songs, then snaps scrolling to top.
    /// </summary>
    private void PopulateList(IEnumerable<SongData> songs)
    {
        // --- Clear out existing items
        foreach (var c in controllers)
            Destroy(c.gameObject);
        controllers.Clear();

        // --- Instantiate new items
        foreach (var s in songs)
        {
            var go   = Instantiate(songItemPrefab, contentParent);
            var ctrl = go.GetComponent<SongItemController>();
            ctrl.manager = this;
            controllers.Add(ctrl);
            ctrl.Initialize(s);
        }

        // --- FORCE layout rebuild so content size is correct
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent);

        // --- Reset scroll to top (normalized + anchoredPosition)
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            // Also zero‐out content offset (in case pivot isn’t 1)
            Vector2 ap = scrollRect.content.anchoredPosition;
            scrollRect.content.anchoredPosition = new Vector2(ap.x, 0f);
        }
    }

    public void FilterSongs(string query)
    {
        var q = (query ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(q))
        {
            PopulateList(allSongs);
            return;
        }
        var filtered = allSongs.Where(s =>
            s.songName.ToLowerInvariant().Contains(q) ||
            s.artist.ToLowerInvariant().Contains(q));
        PopulateList(filtered);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CollapseIfClickedOutside();
    }

    private void CollapseIfClickedOutside()
    {
        if (raycaster == null || eventSystem == null) return;
        var pd      = new PointerEventData(eventSystem) { position = Input.mousePosition };
        var results = new List<RaycastResult>();
        raycaster.Raycast(pd, results);

        if (!results.Any(r => r.gameObject.GetComponentInParent<SongItemController>() != null))
            controllers.ForEach(c => c.AnimateCollapse());
    }

    public void NotifyItemExpanded(SongItemController expanded)
    {
        foreach (var c in controllers)
            if (c != expanded)
                c.Collapse();
    }
}
