// Assets/Scripts/Loading Scene/LoadingSceneController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    [Header("UI References")]
    public Image    bannerImage;
    public TMP_Text songTitleText;
    public TMP_Text artistText;

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
        // 1) Populate UI from SelectedChart
        var song = SelectedChart.Song;
        var bm   = SelectedChart.Beatmap;
        songTitleText.text  = song.songName;
        artistText.text     = song.artist;
        if (song.mainCover != null)
            bannerImage.sprite = song.mainCover;

        // ensure black overlay is hidden
        fadeGroup.alpha = 0f;

        // 2) Kick off load + transition coroutine
        StartCoroutine(DoLoad());
    }

    IEnumerator DoLoad()
    {
        // begin loading but don't switch yet
        var op = SceneManager.LoadSceneAsync(gameplaySceneName);
        op.allowSceneActivation = false;

        float startTime   = Time.unscaledTime;
        Vector3 lastMouse = Input.mousePosition;

        // wait until:
        //  - scene is loaded to 90%,
        //  - AND we've shown at least minDisplayTime,
        //  - AND the mouse has stopped moving
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

        // 3) fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // 4) activate gameplay
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
