using UnityEngine;
using TMPro;

public class SearchBarController : MonoBehaviour
{
    [Tooltip("Drag your TMP_InputField here")]
    public TMP_InputField inputField;

    [Tooltip("Drag your SongListManager here")]
    public SongListManager listManager;

    void Awake()
    {
        // Ensure single‑line enter‑to‑submit
        inputField.lineType = TMP_InputField.LineType.SingleLine;
        inputField.onEndEdit.AddListener(OnSearchFieldSubmit);
    }

    void OnDestroy()
    {
        inputField.onEndEdit.RemoveListener(OnSearchFieldSubmit);
    }

    /// <summary>
    /// Called when user presses Enter or leaves the field.
    /// </summary>
    public void OnSearchFieldSubmit(string query)
    {
        Debug.Log($"[SearchBarController] OnEndEdit with query: '{query}'");
        if (listManager != null)
        {
            listManager.FilterSongs(query);
        }
        else
        {
            Debug.LogError("[SearchBarController] listManager is null!");
        }
    }

    /// <summary>
    /// Hook this to your SearchButton.OnClick()
    /// </summary>
    public void OnSearchButtonClicked()
    {
        var txt = inputField.text;
        Debug.Log($"[SearchBarController] Button clicked, inputField.text: '{txt}'");
        if (listManager != null)
        {
            listManager.FilterSongs(txt);
        }
        else
        {
            Debug.LogError("[SearchBarController] listManager is null!");
        }
    }
}
