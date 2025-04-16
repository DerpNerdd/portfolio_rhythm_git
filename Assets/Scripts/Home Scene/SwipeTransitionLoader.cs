using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SwipeTransitionLoader : MonoBehaviour
{
    [Tooltip("Name of the scene to load after the swipe.")]
    public string sceneName;

    [Tooltip("Full-screen black panel that will swipe up.")]
    public RectTransform transitionPanel;

    [Tooltip("Duration of the swipe animation in seconds.")]
    public float transitionDuration = 1f;

    private float panelHeight;

    void Start()
    {
        if (transitionPanel == null)
        {
            Debug.LogError("SwipeTransitionLoader: transitionPanel is not assigned.");
            return;
        }

        // Calculate panel height from its RectTransform
        panelHeight = transitionPanel.rect.height;

        // Ensure the panel is disabled at start
        transitionPanel.gameObject.SetActive(false);
    }

    // Hook this up to your Button.OnClick
    public void OnPlayButtonClicked()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("SwipeTransitionLoader: sceneName is empty!");
            return;
        }

        StartCoroutine(SwipeUpAndLoad());
    }

    private IEnumerator SwipeUpAndLoad()
    {
        // Activate the panel and position it just below the bottom of the screen
        transitionPanel.gameObject.SetActive(true);
        // Panel pivot is at bottom (0,0.5), anchored to bottom, so anchoredPosition.y = -panelHeight puts it just out of view
        transitionPanel.anchoredPosition = new Vector2(0, -panelHeight);

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            // Move from y = -panelHeight up to y = 0
            float y = Mathf.Lerp(-panelHeight, 0, t);
            transitionPanel.anchoredPosition = new Vector2(0, y);
            yield return null;
        }

        // Make sure it’s fully covering
        transitionPanel.anchoredPosition = Vector2.zero;

        // Finally load the next scene
        SceneManager.LoadScene(sceneName);
    }
}
