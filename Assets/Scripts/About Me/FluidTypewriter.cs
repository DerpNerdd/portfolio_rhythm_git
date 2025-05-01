using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Fluid typewriter effect: reveals text letter-by-letter,
/// with a pause at each word boundary (space/newline).
/// Attach to a TextMeshProUGUI object and call StartTyping() or enable PlayOnStart.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class FluidTypewriter : MonoBehaviour
{
    [Header("Typewriter Settings")]
    [Tooltip("Delay in seconds between each letter reveal.")]
    public float letterDelay = 0.05f;
    [Tooltip("Additional pause in seconds after a word (space or newline).")]
    public float wordPause = 0.25f;
    [Tooltip("If true, automatically starts typing on Start().")]
    public bool  playOnStart = true;

    private TextMeshProUGUI _textComp;
    private string          _fullText;
    private Coroutine       _typingCoroutine;

    void Awake()
    {
        _textComp = GetComponent<TextMeshProUGUI>();
        // Store the full text, then clear it
        _fullText = _textComp.text;
        _textComp.text = string.Empty;
    }

    void Start()
    {
        if (playOnStart)
            StartTyping();
    }

    /// <summary>
    /// Begins the typewriter animation (cancels any in-progress animation).
    /// </summary>
    public void StartTyping()
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);
        _typingCoroutine = StartCoroutine(TypeRoutine());
    }

    IEnumerator TypeRoutine()
    {
        _textComp.text = "";
        foreach (char c in _fullText)
        {
            _textComp.text += c;
            if (c == ' ' || c == '\n')
                yield return new WaitForSeconds(wordPause);
            else
                yield return new WaitForSeconds(letterDelay);
        }
    }
}
