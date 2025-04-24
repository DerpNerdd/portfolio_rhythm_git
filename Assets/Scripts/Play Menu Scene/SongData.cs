// Assets/Scripts/Play Menu Scene/SongData.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BeatmapInfo {
    public string displayName;
    public int    level;
    public Color  color;
    public string mapperName;
    public float    StarDifficulty;
    public float  bpm;     
    public string Source;
    public string Genre;
    public string Language;
    public List<string> Tags;
    public string Submitted;
    public string Ranked;      
    public float  HPDrain;    
    public float  Accuracy;    
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
