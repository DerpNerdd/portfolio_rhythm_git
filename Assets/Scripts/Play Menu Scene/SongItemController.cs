// Assets/Scripts/Play Menu Scene/SongItemController.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class SongItemController : MonoBehaviour
{
    [Header("Icon Settings")]
    [Tooltip("Spacing between icon circles (from HorizontalLayoutGroup)")]
    public float iconSpacing = 8f;

    [Header("Styling")]
    public Image mainUIBackground;
    public Image songBG;

    [Header("Main UI")]
    public Button       mainButton;
    public TMP_Text     songNameText;
    public TMP_Text     artistText;
    public RectTransform iconContainer;
    public GameObject   iconPrefab;

    [Header("Expansion UI")]
    public Transform    difficultyContainer;
    public GameObject   difficultyEntryPrefab;

    [HideInInspector]
    public SongListManager manager;

    private SongData songData;
    private bool     isExpanded = false;
    private LayoutElement diffLayout;
    private RectTransform  diffRect;
    private Coroutine      toggleCoroutine;

    void Awake()
    {
        // Prepare the expand/collapse container
        diffRect   = difficultyContainer as RectTransform;
        diffLayout = difficultyContainer.GetComponent<LayoutElement>()
                   ?? difficultyContainer.gameObject.AddComponent<LayoutElement>();

        difficultyContainer.gameObject.SetActive(false);
        diffLayout.minHeight       = 0;
        diffLayout.preferredHeight = 0;
        diffLayout.flexibleHeight  = 0;

        var hlg = iconContainer.GetComponent<HorizontalLayoutGroup>();
        if (hlg != null) hlg.spacing = iconSpacing;
    }

    /// <summary>
    /// Initialize UI for this song, build icons & difficulty entries.
    /// </summary>
    public void Initialize(SongData data, string groupFilter = null)
    {
        songData = data;

        // Bar tint & thumbnails
        if (mainUIBackground != null) mainUIBackground.color = data.mainColor;
        if (songBG != null && data.coverArt != null)
        {
            songBG.sprite = data.coverArt;
            var c = songBG.color; c.a = 0.5f; songBG.color = c;
        }

        // Text labels
        songNameText.text = data.songName;
        artistText.text   = data.artist;

        // Icons for each matching difficulty
        IEnumerable<BeatmapInfo> toShow = data.beatmaps;
        if (!string.IsNullOrEmpty(groupFilter))
            toShow = toShow.Where(b => b.displayName == groupFilter);

        foreach (var bm in toShow)
        {
            var iconGO = Instantiate(iconPrefab, iconContainer);
            if (iconGO.TryGetComponent<Image>(out var img))
                img.color = bm.color;
        }

        // Build & hook up each difficulty button
        foreach (var bm in toShow)
        {
            var entryGO = Instantiate(difficultyEntryPrefab, difficultyContainer);
            if (entryGO.TryGetComponent<Image>(out var bg))
                bg.color = bm.color;

            entryGO.transform.Find("SongName") .GetComponent<TMP_Text>().text = bm.displayName;
            entryGO.transform.Find("mappedText").GetComponent<TMP_Text>().text = "mapped by";
            entryGO.transform.Find("MapperName").GetComponent<TMP_Text>().text = bm.mapperName;
            entryGO.transform.Find("Difficulty").GetComponent<TMP_Text>().text = bm.level.ToString();

            entryGO.GetComponent<Button>()
                   .onClick.AddListener(() => PlayDifficulty(songData, bm));
        }

        // Expand/collapse on header click
        mainButton.onClick.AddListener(ToggleExpand);
    }

    private void ToggleExpand()
    {
        bool expand = !isExpanded;

        if (expand)
        {
            manager?.NotifyItemExpanded(this);
            // Auto-select first difficulty when expanding via header
            if (songData != null && songData.beatmaps.Count > 0)
                manager?.SelectDifficulty(songData, songData.beatmaps[0]);
        }

        if (toggleCoroutine != null) StopCoroutine(toggleCoroutine);
        toggleCoroutine = StartCoroutine(AnimateToggle(expand));
        isExpanded = expand;
    }

    private IEnumerator AnimateToggle(bool expand)
    {
        if (!difficultyContainer.gameObject.activeSelf)
            difficultyContainer.gameObject.SetActive(true);

        // Measure full height
        diffLayout.enabled = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(diffRect);
        float fullH = LayoutUtility.GetPreferredHeight(diffRect);
        diffLayout.enabled = true;

        float elapsed = 0f, duration = 0.2f;
        float startH = expand ? 0f : fullH;
        float endH   = expand ? fullH : 0f;
        diffLayout.preferredHeight = startH;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            diffLayout.preferredHeight = Mathf.Lerp(startH, endH, elapsed / duration);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            manager?.LayoutRebuild(manager.contentParent);
            yield return null;
        }

        diffLayout.preferredHeight = endH;
        if (!expand) difficultyContainer.gameObject.SetActive(false);
        toggleCoroutine = null;
    }

    /// <summary> Collapse this entry immediately. </summary>
    public void Collapse()
    {
        if (!isExpanded) return;
        if (toggleCoroutine != null) StopCoroutine(toggleCoroutine);

        isExpanded = false;
        diffLayout.preferredHeight = 0;
        difficultyContainer.gameObject.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        manager?.LayoutRebuild(manager.contentParent);
    }

    /// <summary> Plays a difficulty as if clicked. </summary>
    private void PlayDifficulty(SongData song, BeatmapInfo bm)
    {
        manager?.SelectDifficulty(song, bm);
    }

    // ─── NEW API FOR RANDOM PICKER ─────────────────────────────────────────────

    /// <summary> Expose this item’s SongData. </summary>
    public SongData SongData => songData;

    /// <summary> Programmatically expand the entry. </summary>
    public void AnimateExpand()
    {
        if (!isExpanded) ToggleExpand();
    }
}
