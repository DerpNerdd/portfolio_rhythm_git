// Assets/Scripts/Play Menu Scene/SongListManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SongListManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag in the Content RectTransform under ScrollView/Viewport")]
    public RectTransform contentParent;

    [Tooltip("Drag in your SongItem prefab")]
    public GameObject songItemPrefab;

    [Header("Raycasting (optional)")]
    [Tooltip("Drag your Canvas's GraphicRaycaster here, or leave blank to auto‑find.")]
    public GraphicRaycaster raycaster;

    [Tooltip("Drag your EventSystem here, or leave blank to auto‑find.")]
    public EventSystem eventSystem;

    private readonly List<SongItemController> controllers = new();
    private readonly List<SongData> allSongs     = new();

    void Awake()
    {
        // Auto‑find raycaster if not assigned
        if (raycaster == null)
        {
            var canvas = GetComponentInParent<Canvas>();
            raycaster = canvas ? canvas.GetComponent<GraphicRaycaster>() : FindObjectOfType<GraphicRaycaster>();
        }

        // Auto‑find event system
        if (eventSystem == null)
            eventSystem = EventSystem.current ?? FindObjectOfType<EventSystem>();

        if (raycaster == null)
            Debug.LogError("[SongListManager] No GraphicRaycaster found.");
        if (eventSystem == null)
            Debug.LogError("[SongListManager] No EventSystem found.");
    }

    void Start()
    {
        LoadSongs();
        PopulateList();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CheckClickOutside();
    }

    private void CheckClickOutside()
    {
        if (raycaster == null || eventSystem == null) return;

        var pointerData = new PointerEventData(eventSystem) { position = Input.mousePosition };
        var results     = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        // If none of the hits belongs to a SongItemController, collapse all
        foreach (var res in results)
            if (res.gameObject.GetComponentInParent<SongItemController>() != null)
                return;

        CollapseAll();
    }

    private void LoadSongs()
    {
        var songFiles = Resources.LoadAll<TextAsset>("Songs");
        foreach (var file in songFiles)
        {
            var song = JsonUtility.FromJson<SongData>(file.text);
            if (song != null) allSongs.Add(song);
            else Debug.LogWarning($"Failed to parse {file.name}");
        }
    }

    private void PopulateList()
    {
        foreach (var song in allSongs)
        {
            var itemGO = Instantiate(songItemPrefab, contentParent);
            var ctrl   = itemGO.GetComponent<SongItemController>();
            ctrl.manager = this;
            controllers.Add(ctrl);
            ctrl.Initialize(song);
        }
    }

    /// <summary>
    /// Collapse all items with animation when clicking outside.
    /// </summary>
    public void CollapseAll()
    {
        foreach (var ctrl in controllers)
            ctrl.AnimateCollapse();
    }

    /// <summary>
    /// Called by a SongItemController before it expands to collapse its siblings.
    /// </summary>
    public void NotifyItemExpanded(SongItemController expanded)
    {
        foreach (var ctrl in controllers)
            if (ctrl != expanded)
                ctrl.Collapse();
    }
}
