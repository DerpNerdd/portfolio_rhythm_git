// Assets/Scripts/Play Menu Scene/DiffSection1Controller.cs
using System.Linq;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DiffSection1Controller : MonoBehaviour
{
    [Header("Section 1 — Dynamic Fields")]
    [Tooltip("Mapper name")]
    public TMP_Text creatorText;
    [Tooltip("Source")]
    public TMP_Text sourceText;
    [Tooltip("Genre")]
    public TMP_Text genreText;
    [Tooltip("Language")]
    public TMP_Text languageText;
    [Tooltip("Tags (comma-separated)")]
    public TMP_Text tagsText;
    [Tooltip("Date submitted")]
    public TMP_Text submittedText;
    [Tooltip("Date ranked")]
    public TMP_Text rankedText;

    /// <summary>
    /// Populate all Section 1 fields from this beatmap.
    /// </summary>
    public void UpdateSection(BeatmapInfo bm)
    {
        creatorText.text   = bm.mapperName;
        sourceText.text    = bm.Source;
        genreText.text     = bm.Genre;
        languageText.text  = bm.Language;
        tagsText.text      = bm.Tags != null
                               ? string.Join(", ", bm.Tags)
                               : "";
        submittedText.text = bm.Submitted;
        rankedText.text    = bm.Ranked;
    }
}
