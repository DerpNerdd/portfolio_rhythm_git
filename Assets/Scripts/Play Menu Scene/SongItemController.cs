// Assets/Scripts/Play Menu Scene/SongItemController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;      // for Image, LayoutRebuilder, LayoutUtility
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class SongItemController : MonoBehaviour
{
    [Header("Styling")]
    [Tooltip("Background Image on the MainUI bar")]
    public Image mainUIBackground;
    [Tooltip("Semi‑transparent cover art behind text/icons")]
    public Image songBG;

    [Header("Main UI")]
    public Button mainButton;
    public TMP_Text songNameText;
    public TMP_Text artistText;
    public Transform iconContainer;
    public GameObject iconPrefab;

    [Header("Expansion UI")]
    public Transform difficultyContainer;
    public GameObject difficultyEntryPrefab;

    [HideInInspector] public SongListManager manager;

    private bool isExpanded = false;
    private LayoutElement diffLayout;
    private RectTransform diffRect;
    private Coroutine toggleCoroutine;

    void Awake()
    {
        // Prepare for animated height
        diffRect = difficultyContainer as RectTransform;
        diffLayout = difficultyContainer.GetComponent<LayoutElement>()
                   ?? difficultyContainer.gameObject.AddComponent<LayoutElement>();

        difficultyContainer.gameObject.SetActive(false);
        diffLayout.minHeight       = 0;
        diffLayout.preferredHeight = 0;
        diffLayout.flexibleHeight  = 0;
    }

    /// <summary>
    /// Call this to set up coloring, texts, and callbacks.
    /// </summary>
    public void Initialize(SongData data)
    {
        // 1) Tint MainUI bar
        if (mainUIBackground != null)
        {
            Debug.Log($"[SongItem] Applying mainColor {data.mainColor} to {mainUIBackground.gameObject.name}");
            mainUIBackground.color = data.mainColor;
        }

        // 2) Cover art
        if (songBG != null && data.coverArt != null)
        {
            songBG.sprite = data.coverArt;
            var c = songBG.color;
            c.a = 0.5f;
            songBG.color = c;
        }

        // 3) Labels
        songNameText.text = data.songName;
        artistText.text   = data.artist;

        // 4) Difficulty‑icon circles
        foreach (var bm in data.beatmaps)
        {
            var iconGO = Instantiate(iconPrefab, iconContainer);
            if (iconGO.TryGetComponent<Image>(out var img))
            {
                Debug.Log($"[SongItem] Applying beatmap color {bm.color} to icon");
                img.color = bm.color;
            }
        }

        // 5) Difficulty entries
        foreach (var bm in data.beatmaps)
        {
            var entryGO = Instantiate(difficultyEntryPrefab, difficultyContainer);

            // 5a) Tint the entry background
            var entryBg = entryGO.GetComponent<Image>();
            if (entryBg != null)
            {
                Debug.Log($"[SongItem] Applying beatmap color {bm.color} to {entryGO.name}");
                entryBg.color = bm.color;
            }
            else
            {
                // fallback: child named "Image"
                var fallback = entryGO.transform.Find("Image")?
                                     .GetComponent<Image>();
                if (fallback != null)
                {
                    Debug.Log($"[SongItem] Applying beatmap color {bm.color} to child Image");
                    fallback.color = bm.color;
                }
            }

            // 5b) Text fields
            entryGO.transform.Find("SongName")
                   .GetComponent<TMP_Text>().text = bm.displayName;

            entryGO.transform.Find("mappedText")
                   .GetComponent<TMP_Text>().text = "mapped by";

            entryGO.transform.Find("MapperName")
                   .GetComponent<TMP_Text>().text = bm.mapperName;

            entryGO.transform.Find("Difficulty")
                   .GetComponent<TMP_Text>().text = bm.level.ToString();

            // 5c) Play callback
            entryGO.GetComponent<Button>()
                   .onClick.AddListener(() => PlayDifficulty(data, bm));
        }

        // 6) Expand/collapse
        mainButton.onClick.AddListener(ToggleExpand);
    }

    private void ToggleExpand()
    {
        bool willExpand = !isExpanded;
        if (willExpand && manager != null)
            manager.NotifyItemExpanded(this);

        if (toggleCoroutine != null)
            StopCoroutine(toggleCoroutine);

        toggleCoroutine = StartCoroutine(AnimateToggle(willExpand));
        isExpanded = willExpand;
    }

    private IEnumerator AnimateToggle(bool expand)
    {
        if (!difficultyContainer.gameObject.activeSelf)
            difficultyContainer.gameObject.SetActive(true);

        // Measure natural height
        diffLayout.enabled = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(diffRect);
        float fullHeight = LayoutUtility.GetPreferredHeight(diffRect);
        diffLayout.enabled = true;

        // Animate
        float duration = 0.2f, elapsed = 0f;
        float startH = expand ? 0f : fullHeight;
        float endH   = expand ? fullHeight : 0f;
        diffLayout.preferredHeight = startH;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            diffLayout.preferredHeight = Mathf.Lerp(startH, endH, t);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            if (manager != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(manager.contentParent);

            yield return null;
        }

        // Snap and cleanup
        diffLayout.preferredHeight = endH;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        if (manager != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(manager.contentParent);

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
        if (manager != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(manager.contentParent);
    }

    public void AnimateCollapse()
    {
        if (isExpanded)
            ToggleExpand();
    }

    private void PlayDifficulty(SongData song, BeatmapInfo bm)
    {
        Debug.Log($"▶️ Playing {song.songName} [{bm.displayName}]");
        // TODO: load the play scene
    }
}
