using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class AboutMeController : MonoBehaviour
{
    [Header("Model Setup")]
    public Transform modelTransform;        // Assign Derpnerd_Mii here
    public Animator modelAnimator;          // Animator on Derpnerd_Mii

    [Header("Slide Settings")]
    public Vector3 offscreenPosition = new Vector3(-10f, 0f, 5f);
    public Vector3 onScreenPosition    = new Vector3(-2f,  0f, 5f);
    public float   slideDuration       = 1f;

    [Header("Wave Settings")]
    public string  waveTrigger    = "Wave1";
    public float   waveDuration   = 1.5f;    

    [Header("Talk Settings")]
    public string  isTalkingParam = "isTalking";   // Bool parameter in Animator

    [Header("Intro Text")]
    public TextMeshProUGUI greetingText;
    public string        greetingMessage = "Hello! My Name is Alan Sanchez!";
    public TextMeshProUGUI subtitleText;
    public string        subtitleMessage = "Click the buttons to learn more :3.";
    public float         wordDelay       = 0.25f;

    [Header("Voice Audio")]
    public AudioSource voiceAudio;

    [Header("Bubble Buttons")]
    public GameObject[] bubbleButtons;     // Assign each bubble here

    void Start()
    {
        // Initialize UI and bubble states
        greetingText.text = string.Empty;
        subtitleText.text = string.Empty;
        foreach (var b in bubbleButtons)
            b.SetActive(false);

        // Position model offscreen
        modelTransform.position = offscreenPosition;

        // Begin sequence
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        // Slide model into view
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            modelTransform.position = Vector3.Lerp(offscreenPosition, onScreenPosition, t);
            yield return null;
        }

        // Trigger wave animation
        modelAnimator.SetTrigger(waveTrigger);
        yield return new WaitForSeconds(waveDuration);

        // Begin talking state
        modelAnimator.SetBool(isTalkingParam, true);
        if (voiceAudio != null)
            voiceAudio.Play();

        // Typewriter greeting
        yield return TypewriterEffect(greetingText, greetingMessage);
        // Typewriter subtitle
        yield return TypewriterEffect(subtitleText, subtitleMessage);

        // Wait until audio completes
        if (voiceAudio != null)
            yield return new WaitUntil(() => !voiceAudio.isPlaying);

        // End talking state
        modelAnimator.SetBool(isTalkingParam, false);

        // Reveal bubbles
        foreach (var b in bubbleButtons)
            b.SetActive(true);
    }

    IEnumerator TypewriterEffect(TextMeshProUGUI textComp, string fullText)
    {
        string[] words = fullText.Split(' ');
        textComp.text = string.Empty;
        for (int i = 0; i < words.Length; i++)
        {
            if (i > 0)
                textComp.text += " ";
            textComp.text += words[i];
            yield return new WaitForSeconds(wordDelay);
        }
    }
}

public class BubbleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string sceneToLoad;
    private Vector3 originalScale;
    private Coroutine pulseRoutine;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pulseRoutine = StartCoroutine(PulseEffect());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);
        transform.localScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator PulseEffect()
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * 4f;
            float scale = 1f + Mathf.Sin(t) * 0.1f;
            transform.localScale = originalScale * scale;
            yield return null;
        }
    }
}