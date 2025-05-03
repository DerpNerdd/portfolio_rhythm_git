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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // if the newly-loaded scene is *not* Welcome or Home → tear down the music
        if (scene.name != welcomeSceneName && scene.name != homeSceneName)
        {
            // unsub first so we don't get callbacks after destruction
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }
}
