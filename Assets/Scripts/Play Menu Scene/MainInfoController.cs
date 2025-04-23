// Assets/Scripts/Play Menu Scene/MainInfoController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainInfoController : MonoBehaviour
{
    [Header("Background")]
    [Tooltip("Full-screen background image updated per song")]
    public Image backgroundImage;

    [Header("Main Song Display")]
    public Image    songBG;
    public TMP_Text songTitle;
    public TMP_Text songArtist;

    [Header("Difficulty Mini")]
    public Image    diffMiniBG;
    public TMP_Text diffMiniStars;

    [Header("Difficulty Large")]
    public Image    diffLargeBG;

    /// <summary>
    /// Updates all UI elements when a song or difficulty is selected.
    /// </summary>
    public void UpdateMainInfo(SongData song, BeatmapInfo bm)
    {
        // background: swap or clear
        if (backgroundImage != null)
            backgroundImage.sprite = song.mainCover != null
                ? song.mainCover
                : null;

        // thumbnail
        if (songBG != null)
            songBG.sprite = song.coverArt != null
                ? song.coverArt
                : null;

        // text
        songTitle.text  = song.songName;
        songArtist.text = song.artist;

        // tint
        diffMiniBG.color  = bm.color;
        diffLargeBG.color = bm.color;

        // star difficulty
        diffMiniStars.text = bm.StarDifficulty.ToString("F2");
    }
}
