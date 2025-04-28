// Assets/Scripts/Loading Scene/LoadingSceneController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    [Header("Background")]
    [Tooltip("Full-screen background image (mainCover)")]
    public Image backgroundImage;

    [Header("UI References")]
    [Tooltip("Banner (mainCover)")]
    public Image    bannerImage;
    [Tooltip("Song title")]
    public TMP_Text songTitleText;
    [Tooltip("Artist name")]
    public TMP_Text artistText;

    [Header("Additional Fields")]
    [Tooltip("Difficulty name (e.g. Easy, Hard)")]
    public TMP_Text diffNameText;
    [Tooltip("Source field from JSON")]
    public TMP_Text sourceText;
    [Tooltip("Mapper name from JSON")]
    public TMP_Text mapperText;

    [Header("Fade Settings")]
    [Tooltip("CanvasGroup for the black transition (0 = transparent, 1 = opaque)")]
    public CanvasGroup fadeGroup;
    [Tooltip("Speed of fade in/out (units per second)")]
    public float       fadeSpeed       = 2f;

    [Header("Timing")]
    [Tooltip("Minimum time (sec) to show loading screen")]
    public float       minDisplayTime  = 5f;

    [Header("Scenes")]
    [Tooltip("Name of the gameplay scene to load")]
    public string      gameplaySceneName = "GameplayScene";

    void Start()
    {
        // grab selection
        var song = SelectedChart.Song;
        var bm   = SelectedChart.Beatmap;

        // 1) Background (full-screen)
        if (backgroundImage != null && song.mainCover != null)
            backgroundImage.sprite = song.mainCover;

        // 2) Banner, title, artist
        if (song.mainCover != null)
            bannerImage.sprite = song.mainCover;
        songTitleText.text  = song.songName;
        artistText.text     = song.artist;

        // 3) Diff/Source/Mapper
        diffNameText.text   = bm.displayName;
        sourceText.text     = bm.Source;
        mapperText.text     = bm.mapperName;

        // ensure the fade overlay starts transparent
        fadeGroup.alpha = 0f;

        // 4) Kick off load + transition coroutine
        StartCoroutine(DoLoad());
    }

    IEnumerator DoLoad()
    {
        // begin async load
        var op = SceneManager.LoadSceneAsync(gameplaySceneName);
        op.allowSceneActivation = false;

        float startTime   = Time.unscaledTime;
        Vector3 lastMouse = Input.mousePosition;

        // wait for load ≥90%, min display time, and mouse to stop
        while (true)
        {
            bool ready = op.progress >= 0.9f;
            bool timed = Time.unscaledTime - startTime >= minDisplayTime;

            Vector3 now = Input.mousePosition;
            bool moving = now != lastMouse;
            lastMouse = now;

            if (ready && timed && !moving)
                break;

            yield return null;
        }

        // fade out to black
        yield return StartCoroutine(Fade(0f, 1f));

        // activate gameplay
        op.allowSceneActivation = true;
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        fadeGroup.alpha = from;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;
            fadeGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        fadeGroup.alpha = to;
    }
}
