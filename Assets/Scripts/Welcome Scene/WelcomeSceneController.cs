using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class WelcomeSceneController : MonoBehaviour
{
    /* ------------------ Inspector Fields ------------------ */

[Header("References")]
[SerializeField] private TextMeshProUGUI welcomeText; // assign your TMP text here
[SerializeField] private Image flashImage;            // assign your full‑screen Image here

[Header("Text Animation")]
[Tooltip("Maximum character‑spacing to attempt (before clamping to the max width)")]
[SerializeField] private float targetLetterSpacing = 60f;   
[Tooltip("How long (seconds) the text takes to brighten & spread")]
[SerializeField] private float textAnimDuration   = 2f;

[Header("Text Limit")]
[Tooltip("The maximum width (in pixels) the text is allowed to reach")]
[SerializeField] private float maxTextWidth = 800f;

[Header("Flash")]
[Tooltip("Time for the flash fade‑in")]
[SerializeField] private float flashFadeTime      = 0.3f;

[Header("Scene")]
[Tooltip("Exact name of the scene that contains your pink‑circle home")]
[SerializeField] private string homeSceneName     = "Home Scene";

/* ------------------ Life‑cycle ------------------ */

private void Start() 
{
    // Before starting the animation, clamp the letter spacing to prevent the text from exceeding the max width.
    ClampLetterSpacing();
    StartCoroutine(RunSequence());
}

// Calculates the allowed additional spacing based on the max width,
// and adjusts targetLetterSpacing if necessary.
private void ClampLetterSpacing()
{
    // Force a mesh update so preferredWidth is correct.
    welcomeText.ForceMeshUpdate();
    float initialWidth = welcomeText.preferredWidth;
    int charCount = welcomeText.text.Length;
    if (charCount > 1)
    {
        // Calculate the extra space allowed per gap between characters.
        float allowedSpacing = (maxTextWidth - initialWidth) / (charCount - 1);
        // Clamp the target letter spacing if it's too large.
        if (allowedSpacing < targetLetterSpacing)
        {
            targetLetterSpacing = allowedSpacing;
        }
    }
}

/* ------------------ Animation Coroutine ------------------ */

private System.Collections.IEnumerator RunSequence()
{
    // 1) TEXT BRIGHTEN + SPREAD
    float t = 0f;
    Color startColor = new Color(0.53f, 0.53f, 0.53f);  // #888888
    while (t < textAnimDuration)
    {
        t += Time.deltaTime;
        float pct = t / textAnimDuration;
        welcomeText.color = Color.Lerp(startColor, Color.white, pct);
        welcomeText.characterSpacing = Mathf.Lerp(0f, targetLetterSpacing, pct);
        yield return null;
    }

    // 2) FLASH FADE‑IN (flash holds full white until scene transition)
    for (t = 0f; t < flashFadeTime; t += Time.deltaTime)
    {
        float a = t / flashFadeTime;
        flashImage.color = new Color(1f, 1f, 1f, a);
        yield return null;
    }
    flashImage.color = Color.white;  // holds full white

    // Hold the flash for a brief period to give a solid white transition.
    yield return new WaitForSeconds(0.3f);

    // 3) LOAD THE MAIN HOME SCENE
    SceneManager.LoadScene(homeSceneName);
}
}
