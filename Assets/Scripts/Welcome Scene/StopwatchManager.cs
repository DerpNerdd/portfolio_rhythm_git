using UnityEngine;

public class StopwatchManager : MonoBehaviour
{
    public static StopwatchManager Instance { get; private set; }
    public float ElapsedTime { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        ElapsedTime += Time.deltaTime;
    }
}

//end