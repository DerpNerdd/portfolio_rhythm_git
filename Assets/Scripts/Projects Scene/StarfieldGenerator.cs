// StarfieldGenerator.cs
using UnityEngine;

public class StarfieldGenerator : MonoBehaviour
{
    [Tooltip("Prefab with SpriteRenderer + StarTwinkle")]
    public GameObject starPrefab;

    [Tooltip("How many stars to spawn")]
    public int starCount = 200;

    [Tooltip("Rectangular area around the camera to fill")]
    public float width  = 20f;
    public float height = 12f;

    void Start()
    {
        for (int i = 0; i < starCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(-width/2f, width/2f),
                Random.Range(-height/2f, height/2f)
            );
            var star = Instantiate(starPrefab, (Vector3)pos, Quaternion.identity, transform);
            // randomize size & speed a bit
            float scale = Random.Range(0.015f, 0.03f);
            star.transform.localScale = new Vector3(scale, scale, 1f);

            var tw = star.GetComponent<StarTwinkle>();
            if (tw != null)
                tw.twinkleSpeed = Random.Range(0.5f, 2f);
        }
    }
}
