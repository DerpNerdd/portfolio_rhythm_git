using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Alternate About Me controller: slides in model, plays Wave2 and Talk2 animations,
/// types a greeting, then reveals two bubble buttons.
/// </summary>
public class AboutMeController2 : MonoBehaviour
{
    [Header("Model Setup (Alternate)")]
    public Transform introModelTransform;      // Assign Derpnerd_Mii here for alt
    public Animator  introModelAnimator;       // Animator instance for alternate

    [Header("Slide Settings (Alternate)")]
    public Vector3 altOffscreenPosition = new Vector3(-10f, 0f, 5f);
    public Vector3 altOnscreenPosition  = new Vector3(-2f,  0f, 5f);
    public float   altSlideDuration     = 1f;

    [Header("Animation Triggers & Params (Alt)")]
    public string altWaveTrigger       = "Wave2";        // second wave clip
    public float  altWaveDuration      = 1.5f;
    public string altIsTalkingParam    = "isTalking2";   // second talk state

    [Header("Greeting Text (Alternate)")]
    public TextMeshProUGUI greetingText2;
    [TextArea(3,10)]
    public string greetingMessage2 = "Hello! My Name is Alan Sanchez!";
    public float             letterDelay2     = 0.05f;   // new
    public float             wordDelay2       = 0.25f;


    [Header("Voice Audio (Alternate)")]
    public AudioSource       voiceAudio2;

    [Header("Bubble Buttons (2 - Alternate)")]
    [Tooltip("Assign exactly two bubble GameObjects here for alt")] 
    public GameObject[]      bubbleButtons2;

    void Start()
    {
        // clear greeting and hide alt bubbles
        greetingText2.text = string.Empty;
        foreach (var btn in bubbleButtons2)
            btn.SetActive(false);

        // position the alt model offscreen and begin sequence
        introModelTransform.position = altOffscreenPosition;
        StartCoroutine(PlayIntroSequence2());
    }

    IEnumerator PlayIntroSequence2()
    {
        // slide model into view
        float timer2 = 0f;
        while (timer2 < altSlideDuration)
        {
            timer2 += Time.deltaTime;
            float t = Mathf.Clamp01(timer2 / altSlideDuration);
            introModelTransform.position = Vector3.Lerp(altOffscreenPosition, altOnscreenPosition, t);
            yield return null;
        }

        // play alt wave
        introModelAnimator.SetTrigger(altWaveTrigger);
        yield return new WaitForSeconds(altWaveDuration);

        // start alt talk and voice
        introModelAnimator.SetBool(altIsTalkingParam, true);
        if (voiceAudio2 != null)
            voiceAudio2.Play();

        // type greeting only
        yield return TypewriterEffect2(greetingText2, greetingMessage2);

        // wait for voice end
        if (voiceAudio2 != null)
            yield return new WaitUntil(() => !voiceAudio2.isPlaying);

        // end talking and show alt bubbles
        introModelAnimator.SetBool(altIsTalkingParam, false);
        foreach (var btn in bubbleButtons2)
            btn.SetActive(true);
    }

IEnumerator TypewriterEffect2(TextMeshProUGUI txtComp, string message)
{
    txtComp.text = string.Empty;
    foreach (char c in message)
    {
        txtComp.text += c;
        if (c == ' ' || c == '\n')
            yield return new WaitForSeconds(wordDelay2);
        else
            yield return new WaitForSeconds(letterDelay2);
    }
}
}

/// <summary>
/// Handles hover-pulse and click-to-load on bubble GameObjects (alternate).
/// </summary>
public class BubbleButton2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Tooltip("Name of the scene to load when this bubble is clicked.")]
    public string sceneToLoad2;

    private Vector3 originalScale2;
    private Coroutine pulseRoutine2;

    void Awake()
    {
        originalScale2 = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pulseRoutine2 = StartCoroutine(PulseEffect2());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (pulseRoutine2 != null)
            StopCoroutine(pulseRoutine2);
        transform.localScale = originalScale2;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(sceneToLoad2))
            SceneManager.LoadScene(sceneToLoad2);
    }

    IEnumerator PulseEffect2()
    {
        float t2 = 0f;
        while (true)
        {
            t2 += Time.deltaTime * 4f;
            float scale2 = 1f + Mathf.Sin(t2) * 0.1f;
            transform.localScale = originalScale2 * scale2;
            yield return null;
        }
    }
}