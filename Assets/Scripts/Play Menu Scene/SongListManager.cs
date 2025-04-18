using System.Collections.Generic;
using UnityEngine;

public class SongListManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Content Transform under the ScrollRect Viewport")]
    public Transform contentParent;
    [Tooltip("Prefab of a SongItem (with SongItemController)")]
    public GameObject songItemPrefab;

    private List<SongData> allSongs = new List<SongData>();

    void Start()
    {
        LoadSongs();
        PopulateList();
    }

    // 1️⃣ Load all TextAssets under Resources/Songs as JSON
    void LoadSongs()
    {
        TextAsset[] songFiles = Resources.LoadAll<TextAsset>("Songs");
        foreach (var file in songFiles)
        {
            // assumes each file.text is a JSON representation of SongData
            SongData song = JsonUtility.FromJson<SongData>(file.text);
            if (song != null)
                allSongs.Add(song);
            else
                Debug.LogWarning($"Failed to parse {file.name}");
        }
    }

    // 2️⃣ Instantiate one SongItem per SongData
    void PopulateList()
    {
        foreach (var song in allSongs)
        {
            GameObject itemGO = Instantiate(songItemPrefab, contentParent);
            var controller = itemGO.GetComponent<SongItemController>();
            controller.Initialize(song);
        }
    }
}
