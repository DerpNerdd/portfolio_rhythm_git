// Assets/Scripts/Play Menu Scene/SongStatsController.cs
using UnityEngine;
using TMPro;

public class SongStatsController : MonoBehaviour
{
    [Header("Length")]
    public TMP_Text lengthTextNum;

    [Header("BPM")]
    public TMP_Text bpmTextNum;

    [Header("Difficulty")]
    public TMP_Text difficultyNameTitle;

    /// <summary>
    /// Updates song length, BPM & difficulty line.
    /// </summary>
    public void UpdateStats(SongData song, BeatmapInfo bm)
    {
        // LENGTH
        if (song.audioClip != null)
        {
            int tot = Mathf.FloorToInt(song.audioClip.length);
            lengthTextNum.text = $"{tot/60}:{tot%60:00}";
        }
        else lengthTextNum.text = "0:00";

        // BPM (from beatmap JSON now)
        bpmTextNum.text = bm.bpm.ToString("F0");

        // “[4K] Easy mapped by DerpNerd”
        difficultyNameTitle.text =
            $"[4K] {bm.displayName} mapped by {bm.mapperName}";
    }
}
