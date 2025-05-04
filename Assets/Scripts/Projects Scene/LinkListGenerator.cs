using UnityEngine;
using UnityEngine.UI;
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

        foreach (var entry in entries)
        {
            // instantiate a new button under Content
            Button btn = Instantiate(buttonPrefab, contentPanel);
            // set the displayed text
            var txt = btn.GetComponentInChildren<Text>();
            if (txt != null) txt.text = entry.title;

            // capture url for the onClick closure
            string targetURL = entry.url;
            btn.onClick.AddListener(() => Application.OpenURL(targetURL));
        }
    }
}
