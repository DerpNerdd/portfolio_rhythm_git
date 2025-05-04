using UnityEngine;
using UnityEngine.SceneManagement;  // <- for sceneLoaded

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;

    // Names must exactly match your Build Settings scene names:
    [Tooltip("Only in these scenes will the music persist.")]
    public string welcomeSceneName = "Welcome";
    public string homeSceneName    = "Home";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        // subscribe to be notified when any new scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    if (scene.name != welcomeSceneName && scene.name != homeSceneName)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(gameObject);
        return;
    }

    // If we did survive into Home again, make sure we’re still playing:
    var src = GetComponent<AudioSource>();
    if (src != null && !src.isPlaying)
        src.UnPause();
}
}
