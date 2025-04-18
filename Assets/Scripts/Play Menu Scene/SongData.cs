using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BeatmapInfo {
  public string displayName;
  public int level;
  public Color color;
}

[System.Serializable]
public class SongData {
  public string songName;
  public string artist;
  public List<BeatmapInfo> beatmaps;
}
