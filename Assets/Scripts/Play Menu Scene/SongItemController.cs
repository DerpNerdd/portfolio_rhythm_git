using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SongItemController : MonoBehaviour {
    [Header("Main UI")]
    public Button mainButton;
    public TMP_Text songNameText;
    public TMP_Text artistText;
    public Transform iconContainer;
    public GameObject iconPrefab;

    [Header("Expansion UI")]
    public Transform difficultyContainer;
    public GameObject difficultyEntryPrefab;

    bool isExpanded = false;

    public void Initialize(SongData data) {
        // Set texts
        songNameText.text = data.songName;
        artistText.text = data.artist;

        // Create difficulty icons
        foreach (var bm in data.beatmaps) {
            var iconGO = Instantiate(iconPrefab, iconContainer);
            iconGO.GetComponent<Image>().color = bm.color;
        }

        // Create hidden difficulty entries
        foreach (var bm in data.beatmaps) {
            var entryGO = Instantiate(difficultyEntryPrefab, difficultyContainer);
            entryGO.GetComponentInChildren<TMP_Text>().text = $"{bm.displayName} ({bm.level})";
            entryGO.GetComponent<Image>().color = bm.color;
            entryGO.GetComponent<Button>().onClick.AddListener(() => PlayDifficulty(data, bm));
        }

        difficultyContainer.gameObject.SetActive(false);
        mainButton.onClick.AddListener(ToggleExpand);
    }

    void ToggleExpand() {
        isExpanded = !isExpanded;
        difficultyContainer.gameObject.SetActive(isExpanded);
    }

    void PlayDifficulty(SongData song, BeatmapInfo bm) {
        Debug.Log($"Play {song.songName} – {bm.displayName}");
        // TODO: launch play‑scene via your loader
    }
}
