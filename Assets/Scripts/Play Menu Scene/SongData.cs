using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserRating {
    public float Red;
    public float Green;
}

[System.Serializable]
public class RatingSpread {
    public float DarkRedRec;
    public float RedRec;
    public float DarkOrgRec;
    public float OrgRec;
    public float DarkYellowRec;
    public float YellowRec;
    public float DarkGreenRec;
    public float GreenRec;
    public float LightGreenRec;
    public float LightestGreenRec;
}

[System.Serializable]
public class Points {
    public int _300;
    public int _200;
    public int _100;
    public int _50;
    public int X;
}

[System.Serializable]
public class LeaderboardEntry {
    public string Username;
    public int    Combo;
    public float  Accuracy;
    public Points Points;
    public int    Score;
}

[System.Serializable]
public class Leaderboard {
    public LeaderboardEntry First;
    public LeaderboardEntry Second;
    public LeaderboardEntry Third;
}

[System.Serializable]
public class BeatmapInfo {
    public string displayName;
    public int    level;
    public Color  color;
    public string mapperName;
    public float  StarDifficulty;
    public float  bpm;
    public string Source;
    public string Genre;
    public string Language;
    public List<string> Tags;
    public string Submitted;
    public string Ranked;
    public float  HPDrain;
    public float  Accuracy;
    public float  SuccessRate;
    public UserRating   UserRating;
    public RatingSpread RatingSpread;
    public int    TotalPlayCount;
    public int    DifficultyPlayCount;
    public Leaderboard Leaderboard;
    public string chartFile;

}

[System.Serializable]
public class SongData {
    public string             songName;
    public string             artist;
    public Color              mainColor;
    public List<BeatmapInfo>  beatmaps;

    // populated at load-time
    [System.NonSerialized] public Sprite    coverArt;
    [System.NonSerialized] public Sprite    mainCover;
    [System.NonSerialized] public AudioClip audioClip;
    [System.NonSerialized] public string    resourceFolderID;  // <--- new
}
