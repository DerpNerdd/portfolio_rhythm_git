// Assets/Scripts/Play Menu Scene/SongItemController.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;      // for Image, LayoutRebuilder, LayoutUtility, HorizontalLayoutGroup
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

    private bool isExpanded = false;
    private LayoutElement diffLayout;
    private RectTransform diffRect;
    private Coroutine toggleCoroutine;

    void Awake()
    {
        // prepare the difficulty container for animated height
        diffRect = difficultyContainer as RectTransform;
        diffLayout = difficultyContainer.GetComponent<LayoutElement>()
                   ?? difficultyContainer.gameObject.AddComponent<LayoutElement>();

        difficultyContainer.gameObject.SetActive(false);
        diffLayout.minHeight = diffLayout.preferredHeight = diffLayout.flexibleHeight = 0;

        // apply icon spacing if a HorizontalLayoutGroup is present
        var hlg = iconContainer.GetComponent<HorizontalLayoutGroup>();
        if (hlg != null)
            hlg.spacing = iconSpacing;
    }

    /// <summary>
    /// Initialize this item. If groupFilter != null, only show that difficulty.
    /// </summary>
    public void Initialize(SongData data, string groupFilter = null)
    {
        // 1) Tint the main UI bar
        if (mainUIBackground != null)
            mainUIBackground.color = data.mainColor;

        // 2) Set cover art background
        if (songBG != null && data.coverArt != null)
        {
            songBG.sprite = data.coverArt;
            var c = songBG.color;
            c.a = 0.5f;
            songBG.color = c;
        }

        // 3) Update labels
        songNameText.text = data.songName;
        artistText.text   = data.artist;

        // Determine which beatmaps to show
        IEnumerable<BeatmapInfo> toShow = data.beatmaps;
        if (!string.IsNullOrEmpty(groupFilter))
            toShow = toShow.Where(b => b.displayName == groupFilter);

        // 4) Instantiate icon circles
        foreach (var bm in toShow)
        {
            var iconGO = Instantiate(iconPrefab, iconContainer);
            if (iconGO.TryGetComponent<Image>(out var img))
                img.color = bm.color;
        }

        // 5) Instantiate difficulty entries
        foreach (var bm in toShow)
        {
            var entryGO = Instantiate(difficultyEntryPrefab, difficultyContainer);

            // tint entry background
            if (entryGO.TryGetComponent<Image>(out var entryBg))
                entryBg.color = bm.color;

            // set text fields
            entryGO.transform.Find("SongName").GetComponent<TMP_Text>().text   = bm.displayName;
            entryGO.transform.Find("mappedText").GetComponent<TMP_Text>().text = "mapped by";
            entryGO.transform.Find("MapperName").GetComponent<TMP_Text>().text = bm.mapperName;
            entryGO.transform.Find("Difficulty").GetComponent<TMP_Text>().text = bm.level.ToString();

            // wire up selection callback
            entryGO.GetComponent<Button>().onClick.AddListener(() => PlayDifficulty(data, bm));
        }

        // 6) Hook expand/collapse
        mainButton.onClick.AddListener(ToggleExpand);
    }

    private void ToggleExpand()
    {
        bool expand = !isExpanded;
        if (expand && manager != null)
            manager.NotifyItemExpanded(this);

        if (toggleCoroutine != null)
            StopCoroutine(toggleCoroutine);

        toggleCoroutine = StartCoroutine(AnimateToggle(expand));
        isExpanded = expand;
    }

    private IEnumerator AnimateToggle(bool expand)
    {
        if (!difficultyContainer.gameObject.activeSelf)
            difficultyContainer.gameObject.SetActive(true);

        // measure target height
        diffLayout.enabled = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(diffRect);
        float full = LayoutUtility.GetPreferredHeight(diffRect);
        diffLayout.enabled = true;

        // animate
        float elapsed = 0f, duration = 0.2f;
        float startH  = expand ? 0f : full;
        float endH    = expand ? full : 0f;
        diffLayout.preferredHeight = startH;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            diffLayout.preferredHeight = Mathf.Lerp(startH, endH, elapsed / duration);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            manager?.LayoutRebuild(manager.contentParent);

            yield return null;
        }

        // finalize
        diffLayout.preferredHeight = endH;
        if (!expand)
            difficultyContainer.gameObject.SetActive(false);

        toggleCoroutine = null;
    }

    public void Collapse()
    {
        if (!isExpanded) return;
        if (toggleCoroutine != null)
            StopCoroutine(toggleCoroutine);

        isExpanded = false;
        diffLayout.preferredHeight = 0;
        difficultyContainer.gameObject.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        manager?.LayoutRebuild(manager.contentParent);
    }

    public void AnimateCollapse() => ToggleExpand();

    private void PlayDifficulty(SongData song, BeatmapInfo bm)
    {
        // update the right-hand stats panel
        manager?.SelectDifficulty(song, bm);
        Debug.Log($"▶️ Selected {song.songName} – {bm.displayName}");
    }
}
