using UnityEngine;
using TMPro;

/// <summary>
/// Updates the play‐count section with total and per‐difficulty counts.
/// </summary>
public class SongPlayStatsController : MonoBehaviour
{
    [Header("Play Counts")]
    [Tooltip("Text for total play count")]
    public TMP_Text totalPlayCountText;
    [Tooltip("Text for this difficulty's play count")]
    public TMP_Text diffPlayCountText;

    /// <summary>
    /// Call this with the currently selected beatmap info.
    /// </summary>
    public void UpdatePlayStats(BeatmapInfo bm)
    {
        // Assumes bm.TotalPlayCount and bm.DifficultyPlayCount are ints
        totalPlayCountText.text = bm.TotalPlayCount.ToString("N0");
        diffPlayCountText.text  = bm.DifficultyPlayCount.ToString("N0");
    }
}