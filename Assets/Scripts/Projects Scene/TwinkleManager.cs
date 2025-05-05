// TwinkleManager.cs
using System.Collections.Generic;
using UnityEngine;

public class TwinkleManager : MonoBehaviour
{
    public static TwinkleManager Instance { get; private set; }

    [Tooltip("How often (sec) to update twinkle on all registered links.")]
    public float tickInterval = 0.1f; // 10 Hz

    float nextTick;
    readonly List<LinkTextTMPEffect> links = new();

    void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }

    void Update()
    {
        if (Time.time < nextTick) return;
        nextTick = Time.time + tickInterval;

        float t = Time.time;
        for (int i = links.Count - 1; i >= 0; i--)
        {
            var link = links[i];
            if (link == null) links.RemoveAt(i);
            else link.Tick(t);
        }
    }

    public void Register(LinkTextTMPEffect link)   => links.Add(link);
    public void Unregister(LinkTextTMPEffect link) => links.Remove(link);
}
