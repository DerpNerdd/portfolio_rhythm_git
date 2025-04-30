using UnityEngine;

/// <summary>
/// Applies a subtle perlin-noise-based vertex-free jiggle to a bubble sphere.
/// Attach to your Bubble GameObject (inner sphere and its glow child).
/// </summary>
public class BubbleJiggle : MonoBehaviour
{
    [Tooltip("Maximum relative scale deviation (e.g. 0.1 = ±10% per axis)")]
    public float amplitude = 0.1f;
    [Tooltip("Speed multiplier for the noise animation")]
    public float speed = 1f;

    private Vector3 baseScale;
    private Vector3 noiseSeed;

    void Start()
    {
        // Cache original scale and pick random offsets for each axis
        baseScale = transform.localScale;
        noiseSeed = new Vector3(
            Random.Range(0f, 100f),
            Random.Range(0f, 100f),
            Random.Range(0f, 100f)
        );
    }

    void Update()
    {
        float t = Time.time * speed;
        // Per-axis noise from -0.5..0.5, scaled by 2*amplitude
        float nx = (Mathf.PerlinNoise(noiseSeed.x, t) - 0.5f) * 2f * amplitude;
        float ny = (Mathf.PerlinNoise(noiseSeed.y, t) - 0.5f) * 2f * amplitude;
        float nz = (Mathf.PerlinNoise(noiseSeed.z, t) - 0.5f) * 2f * amplitude;

        // Apply jittered scale
        transform.localScale = new Vector3(
            baseScale.x * (1 + nx),
            baseScale.y * (1 + ny),
            baseScale.z * (1 + nz)
        );
    }
}
