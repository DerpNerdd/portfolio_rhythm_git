using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Populates the top-3 leaderboard UI entries from BeatmapInfo.Leaderboard,
/// with safe fallbacks if data is missing.
/// </summary>
public class LeaderboardsController : MonoBehaviour
{
    [System.Serializable]
    public class LeaderboardUIEntry
    {
        [Tooltip("Position # text (1, 2, 3)")]
        public TMP_Text positionText;
        [Tooltip("Player username")]
        public TMP_Text usernameText;
        [Tooltip("Combo count")]
        public TMP_Text comboText;
        [Tooltip("Accuracy %")]
        public TMP_Text accuracyText;
        [Tooltip("Points breakdown text")]
        public TMP_Text pointsText;    // formatted "300/200/100/50/X"
        [Tooltip("Total score")]
        public TMP_Text scoreText;
    }

    [Tooltip("First place UI entry")]
    public LeaderboardUIEntry firstEntry;
    [Tooltip("Second place UI entry")]
    public LeaderboardUIEntry secondEntry;
    [Tooltip("Third place UI entry")]
    public LeaderboardUIEntry thirdEntry;

    /// <summary>
    /// Updates the three leaderboard entries, using defaults if bm.Leaderboard or any entry is null.
    /// </summary>
    public void UpdateLeaderboards(BeatmapInfo bm)
    {
        // Always populate all three, even if bm.Leaderboard is null
        Populate(firstEntry,  "1", bm.Leaderboard?.First);
        Populate(secondEntry, "2", bm.Leaderboard?.Second);
        Populate(thirdEntry,  "3", bm.Leaderboard?.Third);
    }

    void Populate(LeaderboardUIEntry ui, string pos, LeaderboardEntry data)
    {
        ui.positionText.text = pos;

        if (data != null)
        {
            ui.usernameText .text = string.IsNullOrEmpty(data.Username) ? "User" : data.Username;
            ui.comboText    .text = data.Combo.ToString("N0");
            ui.accuracyText .text = data.Accuracy.ToString("F2") + "%";

            // Safe‐navigate into Points, defaulting each segment to 0
            var pts = data.Points;
            int p300 = pts?._300 ?? 0;
            int p200 = pts?._200 ?? 0;
            int p100 = pts?._100 ?? 0;
            int p50  = pts?._50  ?? 0;
            int pX   = pts?.X    ?? 0;
            ui.pointsText   .text = $"{p300}/{p200}/{p100}/{p50}/{pX}";

            ui.scoreText    .text = data.Score.ToString("N0");
        }
        else
        {
            // Fallback defaults
            ui.usernameText .text = "User";
            ui.comboText    .text = "0";
            ui.accuracyText .text = "0.00%";
            ui.pointsText   .text = "0/0/0/0/0";
            ui.scoreText    .text = "0";
        }
    }
}
