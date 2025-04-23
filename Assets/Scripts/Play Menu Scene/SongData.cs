// Assets/Scripts/Play Menu Scene/SongData.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BeatmapInfo {
    public string displayName;
    public int    level;
    public Color  color;
    public string mapperName;
    public int    StarDifficulty;
    public float  bpm;            // ← matches your JSON `"bpm": 115`
}

[System.Serializable]
public class SongData {
    public string           songName;
    public string           artist;
    public Color            mainColor;
    public List<BeatmapInfo> beatmaps;

    // loaded at runtime
    [System.NonSerialized] public Sprite    coverArt;
    [System.NonSerialized] public AudioClip audioClip;
    [System.NonSerialized] public Sprite    mainCover;
}
