// Assets/Scripts/Play Menu Scene/SongData.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BeatmapInfo {
    public string displayName;
    public int    level;
    public Color  color;
    public string mapperName;
    public int    starDifficulty;
}
[System.Serializable]
public class SongData {
    public string        songName;
    public string        artist;
    public Color         mainColor;   // <-- new
    public List<BeatmapInfo> beatmaps;

    // populated at load-time
    [System.NonSerialized] public Sprite     coverArt;
    [System.NonSerialized] public AudioClip  audioClip;
}
