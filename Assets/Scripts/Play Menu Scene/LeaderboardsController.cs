using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Populates the top-3 leaderboard UI entries from BeatmapInfo.Leaderboard.
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
    /// Updates the three leaderboard entries.
    /// </summary>
    public void UpdateLeaderboards(BeatmapInfo bm)
    {
        if (bm.Leaderboard == null) return;
        Populate(firstEntry,  "1", bm.Leaderboard.First);
        Populate(secondEntry, "2", bm.Leaderboard.Second);
        Populate(thirdEntry,  "3", bm.Leaderboard.Third);
    }

    void Populate(LeaderboardUIEntry ui, string pos, LeaderboardEntry data)
    {
        ui.positionText .text = pos;
        ui.usernameText .text = data.Username;
        ui.comboText    .text = data.Combo.ToString("N0");
        ui.accuracyText .text = data.Accuracy.ToString("F2") + "%";
        ui.pointsText   .text = $"{data.Points._300}/{data.Points._200}/{data.Points._100}/{data.Points._50}/{data.Points.X}";
        ui.scoreText    .text = data.Score.ToString("N0");
    }
}