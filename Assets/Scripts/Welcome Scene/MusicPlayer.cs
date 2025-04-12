using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Singleton pattern to ensure one MusicPlayer exists.
    public static MusicPlayer instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Make MusicPlayer persist across scene changes.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy any additional MusicPlayer objects.
            Destroy(gameObject);
        }
    }
}
