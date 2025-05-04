// StarTwinkle.cs
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StarTwinkle : MonoBehaviour
{
    [Tooltip("How fast each star fades in/out")]
    public float twinkleSpeed = 1f;

    [Tooltip("Minimum alpha (dim)")]
    [Range(0f,1f)]
    public float minAlpha = 0.2f;

    private SpriteRenderer sr;
    private float phase;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // randomize the starting point, so they don't all pulse in unison:
        phase = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // Simple sine-wave alpha oscillation
        float a = Mathf.Lerp(minAlpha, 1f,
                    (Mathf.Sin(Time.time * twinkleSpeed + phase) + 1f) * 0.5f);
        var c = sr.color;
        c.a = a;
        sr.color = c;
    }
}
