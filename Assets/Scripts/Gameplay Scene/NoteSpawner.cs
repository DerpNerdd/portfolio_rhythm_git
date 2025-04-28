// Assets/Scripts/Gameplay Scene/NoteSpawner.cs
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("UI Refs")]
    public RectTransform noteContainer;
    public RectTransform lanesContainer;
    public GameObject    notePrefab;

    private RectTransform[] _lanes;

    void Awake()
    {
        int n = lanesContainer.childCount;
        _lanes = new RectTransform[n];
        for (int i = 0; i < n; i++)
            _lanes[i] = lanesContainer.GetChild(i) as RectTransform;
    }

    /// <summary>
    /// Call this once per note in your chart.
    /// laneIndex is 0–3, yPos is whatever your timing→pixel script spits out.
    /// </summary>
    public RectTransform SpawnNote(int laneIndex, float yPos)
    {
        // 1) Instantiate under the noteContainer
        var go = Instantiate(notePrefab, noteContainer);
        var rt = go.GetComponent<RectTransform>();

        // 2) Force both anchors to bottom‐left
        rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
        rt.pivot     = new Vector2(0.5f, 0.5f);

        // 3) Compute the X‐position: center of the lane rect
        var lane = _lanes[laneIndex];
        float laneCenterX = lane.anchoredPosition.x + lane.rect.width * 0.5f;

        // 4) Finally place the note
        rt.anchoredPosition = new Vector2(laneCenterX, yPos);

        return rt;
    }
}
