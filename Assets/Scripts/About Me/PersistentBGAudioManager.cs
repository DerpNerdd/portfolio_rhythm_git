using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to the BGAudio GameObject. Plays and loops background music,
/// persists only across the specified scenes, and destroys itself when
/// navigating to other scenes.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PersistentBGAudioManager : MonoBehaviour
{
    [Header("Scenes to persist audio through")]
    [Tooltip("Names of scenes where background audio should continue playing.")]
    public List<string> persistentSceneNames = new List<string>();

    private static PersistentBGAudioManager _instance;
    private AudioSource _audioSource;

    void Awake()
    {
        // Singleton: if an instance already exists, destroy duplicate
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set up the singleton and prevent destruction on scene load
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure the AudioSource loops
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
    }

    void OnEnable()
    {
        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the newly loaded scene is not in our list, destroy this audio manager
        if (_instance == this && !persistentSceneNames.Contains(scene.name))
        {
            Destroy(gameObject);
        }
    }
}
