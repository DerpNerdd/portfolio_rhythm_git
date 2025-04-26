// Assets/Scripts/Play Menu Scene/PlayButtonController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButtonController : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnPlayClicked);
    }

    void OnDestroy()
    {
        _button.onClick.RemoveListener(OnPlayClicked);
    }

    private void OnPlayClicked()
    {
        // ensure something’s selected
        if (SelectedChart.Song == null || SelectedChart.Beatmap == null)
        {
            Debug.LogWarning("No song or difficulty selected!");
            return;
        }

        // kick off our smooth loading
        SceneManager.LoadScene("Loading Scene");
    }
}
