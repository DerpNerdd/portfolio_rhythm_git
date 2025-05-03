using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Synchronizes UI (title & description) with the ModelCarousel selection.
/// </summary>
public class ModelCarouselUI : MonoBehaviour
{
    [Header("Carousel Reference")]
    [Tooltip("Drag in the GameObject with ModelCarousel attached.")]
    public ModelCarousel carousel;

    [Header("UI Elements")]
    [Tooltip("TextMeshProUGUI for the title above the model.")]
    public TextMeshProUGUI titleText;
    [Tooltip("TextMeshProUGUI for the description below the model.")]
    public TextMeshProUGUI descriptionText;

    [Header("Model Data")]
    [Tooltip("Titles corresponding to each model, in the same order as the carousel's models list.")]
    public List<string> titles = new List<string>();
    [Tooltip("Descriptions corresponding to each model.")]
    public List<string> descriptions = new List<string>();

    void Start()
    {
        if (carousel == null)
        {
            Debug.LogError("ModelCarouselUI: Carousel reference not set.");
            enabled = false;
            return;
        }
        // Subscribe to index changes
        carousel.OnSelectionChanged += UpdateUI;
        // Initialize display
        UpdateUI(carousel.currentIndex);
    }

    void OnDestroy()
    {
        if (carousel != null)
            carousel.OnSelectionChanged -= UpdateUI;
    }

    /// <summary>
    /// Updates the UI texts based on the selected index.
    /// </summary>
    void UpdateUI(int index)
    {
        if (titleText != null)
        {
            titleText.text = (index >= 0 && index < titles.Count) ? titles[index] : string.Empty;
        }
        if (descriptionText != null)
        {
            descriptionText.text = (index >= 0 && index < descriptions.Count) ? descriptions[index] : string.Empty;
        }
    }
}
