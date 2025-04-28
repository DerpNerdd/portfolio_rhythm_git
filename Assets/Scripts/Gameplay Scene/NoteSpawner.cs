// Assets/Scripts/Gameplay Scene/NoteSpawner.cs
using UnityEngine;
using UnityEngine.UI;   // <-- for Image

public class NoteSpawner : MonoBehaviour
{
    [Header("UI Refs")]
    public RectTransform lanesContainer;
    public GameObject    notePrefab;

    [Header("Lane Note Sprites (0=left,1=down,2=up,3=right)")]
    public Sprite[]      laneNoteSprites;

    private RectTransform[] _lanes;

    void Awake()
    {
        int n = lanesContainer.childCount;
        _lanes = new RectTransform[n];
        for (int i = 0; i < n; i++)
            _lanes[i] = lanesContainer.GetChild(i) as RectTransform;
    }

    public RectTransform SpawnNote(int laneIndex, float yPos)
    {
        var lane = _lanes[laneIndex];
        var go   = Instantiate(notePrefab, lane, false);
        var rt   = go.GetComponent<RectTransform>();

        // stretch across lane, pivot top-center (same as before)…
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.offsetMin = new Vector2(0, rt.offsetMin.y);
        rt.offsetMax = new Vector2(0, rt.offsetMax.y);
        rt.anchoredPosition = new Vector2(0, yPos);

        // ***** NEW: swap in the correct sprite *****
        if (laneNoteSprites != null &&
            laneIndex < laneNoteSprites.Length)
        {
            // assumes your prefab has an Image on the root
            var img = go.GetComponent<Image>();
            if (img != null)
            {
                img.sprite        = laneNoteSprites[laneIndex];
                img.preserveAspect = true;
            }
            else
            {
                Debug.LogWarning("NoteSpawner: notePrefab missing Image component!");
            }
        }

        return rt;
    }
}
