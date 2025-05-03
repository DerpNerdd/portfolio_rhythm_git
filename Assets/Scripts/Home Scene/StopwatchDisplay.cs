using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class StopwatchDisplay : MonoBehaviour
{
    TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
        if (_text == null)
            Debug.LogError("StopwatchDisplay needs a TMP_Text!", this);
    }

    void Update()
    {
        if (StopwatchManager.Instance == null) return;

        float t = StopwatchManager.Instance.ElapsedTime;
        int hours   = (int)(t / 3600f);
        int minutes = (int)((t % 3600f) / 60f);
        int seconds = (int)(t % 60f);

        _text.text = $"running {hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}
