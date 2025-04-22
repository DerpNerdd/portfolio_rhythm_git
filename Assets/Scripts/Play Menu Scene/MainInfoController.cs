// Assets/Scripts/Play Menu Scene/MainInfoController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainInfoController : MonoBehaviour
{
    [Header("Main Song Display")]
    public Image    songBG;           // SongBG Image
    public TMP_Text songTitle;        // SongTitle text
    public TMP_Text songArtist;       // SongArtist text

    [Header("Difficulty Mini")]
    public Image    diffMiniBG;       // SongDiffMini image
    public TMP_Text diffMiniStars;    // SongDiffMini/Text
    // public Image starIcon;         // optional, your star image

    [Header("Difficulty Large")]
    public Image    diffLargeBG;      // SongDiffLarge image

    /// <summary>
    /// Call this whenever a new difficulty is selected.
    /// </summary>
    public void UpdateMainInfo(SongData song, BeatmapInfo bm)
    {
        // 1. big cover
        if (song.coverArt != null)
            songBG.sprite = song.coverArt;

        // 2. texts
        songTitle.text  = song.songName;
        songArtist.text = song.artist;

        // 3. difficulty tint
        diffMiniBG.color  = bm.color;
        diffLargeBG.color = bm.color;

        // 4. star count
        diffMiniStars.text = bm.starDifficulty.ToString();
    }
}
