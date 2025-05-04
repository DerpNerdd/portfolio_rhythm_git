// Assets/Scripts/WebGLAudioUnlocker.cs
using UnityEngine;
public class WebGLAudioUnlocker : MonoBehaviour
{
    bool _unlocked = false;
    void Update()
    {
        if (!_unlocked && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            // Touch the AudioContext
            var a = gameObject.AddComponent<AudioSource>();
            a.playOnAwake = false;
            a.clip = AudioClip.Create("silence", 1, 1, 44100, false);
            a.Play();
            _unlocked = true;
        }
    }
}
