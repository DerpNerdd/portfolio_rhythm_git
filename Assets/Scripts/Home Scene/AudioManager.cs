using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class HomeSceneAudioManager : MonoBehaviour
{
    [Tooltip("Must match your Home Scene name in Build Settings")]
    public string homeSceneName = "Home";

    AudioSource _src;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
        // make sure it's off at start if persistent music exists
        if (MusicPlayer.instance != null)
            _src.enabled = false;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != homeSceneName) return;

        if (MusicPlayer.instance != null)
        {
            // persistent Welcome→Home music still alive → disable local
            if (_src.isPlaying) _src.Stop();
            _src.enabled = false;
        }
        else
        {
            // no persistent music → turn this AudioSource back on
            _src.enabled = true;
            if (!_src.isPlaying)
                _src.Play();
        }
    }
}
