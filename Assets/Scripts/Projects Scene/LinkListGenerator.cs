using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class LinkListGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct LinkEntry
    {
        public string title;
        public string url;
    }

    [Tooltip("Drag your LinkScrollView/Viewport/Content RectTransform here")]
    public RectTransform contentPanel;

    [Tooltip("Drag your LinkButton prefab here")]
    public Button buttonPrefab;

    [Tooltip("Define each clickable title + URL")]
    public List<LinkEntry> entries = new List<LinkEntry>();

    void Start()
    {
        if (contentPanel == null || buttonPrefab == null)
        {
            Debug.LogError("LinkListGenerator: assign contentPanel and buttonPrefab.");
            return;
        }

        // (Optional) clear out any existing buttons
        foreach (Transform child in contentPanel) Destroy(child.gameObject);

        foreach (var entry in entries)
        {
            // 1) Spawn the prefab under Content
            Button btn = Instantiate(buttonPrefab, contentPanel);

            // 2) Populate its TMP label
            TMP_Text tmp = btn.GetComponentInChildren<TMP_Text>();
            if (tmp != null) tmp.text = entry.title;
            else Debug.LogWarning("LinkListGenerator: No TMP_Text found on prefab!");

            // 3) Force it to resize itself
            var dyn = btn.GetComponent<DynamicButtonLayout>();
            if (dyn != null) dyn.UpdateLayout();

            // 4) Normalize URL and hook up the click
            string targetURL = entry.url?.Trim() ?? "";
            if (!targetURL.StartsWith("http://") && !targetURL.StartsWith("https://"))
                targetURL = "https://" + targetURL;

            Debug.Log($"LinkListGenerator: Opening URL → {targetURL}");
            btn.onClick.AddListener(() => Application.OpenURL(targetURL));
        }
    }
}
