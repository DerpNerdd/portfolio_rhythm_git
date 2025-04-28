// Assets/Scripts/HitLineManager.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class HitLineManager : MonoBehaviour
{
    [Tooltip("The same LanesContainer you use for notes")]
    public RectTransform lanesContainer;

    [Tooltip("Y position (in local coords) where arrows should sit")]
    public float hitY;

    [Tooltip("Sprites for each lane arrow (0=left,1=down,2=up,3=right)")]
    public Sprite[] arrowSprites = new Sprite[4];

    void Start()
    {
        // 1) Hide the old HitLine (if you still have a reference)
        //    You said you already disabled it manually, so just move on.

        // 2) Build the 4 static arrows
        BuildHitArrows();
    }

    void BuildHitArrows()
    {
        // Clear any existing hit-arrows
        foreach (Transform child in transform)
            if (child.name.StartsWith("HitArrow_"))
                Destroy(child.gameObject);

        int count = Mathf.Min(lanesContainer.childCount, arrowSprites.Length);

        for (int i = 0; i < count; i++)
        {
            var lane = lanesContainer.GetChild(i) as RectTransform;
            if (lane == null) continue;

            // Create arrow GameObject
            var go = new GameObject($"HitArrow_{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(lane, false);

            // Position at center-X of lane, at hitY
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot     = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0, hitY);

            // Size: 80% of lane width, 20% of lane height (tweak as you like)
            float w = lane.rect.width  * 0.8f;
            float h = lane.rect.height * 0.2f;
            rt.sizeDelta = new Vector2(w, h);

            // Assign your arrow sprite
            var img = go.GetComponent<Image>();
            img.sprite        = arrowSprites[i];
            img.preserveAspect = true;
            img.raycastTarget  = false;
        }
    }
}
