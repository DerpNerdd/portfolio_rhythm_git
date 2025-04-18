// Assets/Scripts/Play Menu Scene/SongItemController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;      // for LayoutRebuilder and LayoutUtility
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class SongItemController : MonoBehaviour
{
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
        // Ensure we can animate height
        diffRect = difficultyContainer as RectTransform;
        diffLayout = difficultyContainer.GetComponent<LayoutElement>();
        if (diffLayout == null)
            diffLayout = difficultyContainer.gameObject.AddComponent<LayoutElement>();

        // Start collapsed
        difficultyContainer.gameObject.SetActive(false);
        diffLayout.minHeight = diffLayout.preferredHeight = 0;
        diffLayout.flexibleHeight = 0;
    }

    public void Initialize(SongData data)
    {
        // Fill in text fields
        songNameText.text = data.songName;
        artistText.text  = data.artist;

        // Create difficulty icons
        foreach (var bm in data.beatmaps)
        {
            var iconGO = Instantiate(iconPrefab, iconContainer);
            iconGO.GetComponent<Image>().color = bm.color;
        }

        // Create difficulty entries
        foreach (var bm in data.beatmaps)
        {
            var entryGO = Instantiate(difficultyEntryPrefab, difficultyContainer);
            entryGO.GetComponentInChildren<TMP_Text>().text = $"{bm.displayName} ({bm.level})";
            entryGO.GetComponent<Image>().color = bm.color;
            entryGO.GetComponent<Button>().onClick.AddListener(() => PlayDifficulty(data, bm));
        }

        // Wire up expand/collapse
        mainButton.onClick.AddListener(ToggleExpand);
    }

    private void ToggleExpand()
    {
        bool willExpand = !isExpanded;

        // Before expanding, collapse others
        if (willExpand && manager != null)
            manager.NotifyItemExpanded(this);

        // Stop any ongoing animation
        if (toggleCoroutine != null)
            StopCoroutine(toggleCoroutine);

        // Start expand/collapse animation
        toggleCoroutine = StartCoroutine(AnimateToggle(willExpand));
        isExpanded = willExpand;
    }

    private IEnumerator AnimateToggle(bool expand)
    {
        // Make sure container is active so children get laid out
        if (!difficultyContainer.gameObject.activeSelf)
            difficultyContainer.gameObject.SetActive(true);

        // Measure natural height
        diffLayout.enabled = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(diffRect);
        float fullHeight = LayoutUtility.GetPreferredHeight(diffRect);
        diffLayout.enabled = true;

        // Animate from 0 → fullHeight or vice versa
        float duration = 0.2f;
        float elapsed = 0f;
        float startH = expand ? 0f : fullHeight;
        float endH   = expand ? fullHeight : 0f;
        diffLayout.preferredHeight = startH;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            diffLayout.preferredHeight = Mathf.Lerp(startH, endH, t);

            // Rebuild this item and parent content
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            if (manager != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(manager.contentParent);

            yield return null;
        }

        // Snap to final size
        diffLayout.preferredHeight = endH;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        if (manager != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(manager.contentParent);

        // Hide if collapsed
        if (!expand)
            difficultyContainer.gameObject.SetActive(false);

        toggleCoroutine = null;
    }

    /// <summary>
    /// Immediately collapse this item without animation.
    /// </summary>
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

    /// <summary>
    /// Smoothly collapse via the same animation.
    /// </summary>
    public void AnimateCollapse()
    {
        if (isExpanded)
            ToggleExpand();
    }

    private void PlayDifficulty(SongData song, BeatmapInfo bm)
    {
        Debug.Log($"▶️ Playing {song.songName} [{bm.displayName}]");
        // TODO: load your play scene here
    }
}
