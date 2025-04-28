// Assets/Scripts/Gameplay Scene/NoteController.cs
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class NoteController : MonoBehaviour
{
    // Exposed for hit‐detection in RhythmGameManager
    public int laneIndex { get; private set; }
    public float noteTime  { get; private set; }
    public bool handled     { get; set; }

    private float approachTime;
    private float spawnY;
    private float hitY;
    private RhythmGameManager gm;
    private RectTransform rt;

    /// <summary>
    /// Initialize this note with timing, lane, and reference info.
    /// </summary>
    public void Init(int laneIndex,
                     float noteTime,
                     float approachTime,
                     float spawnY,
                     float hitY,
                     RhythmGameManager gm)
    {
        this.laneIndex     = laneIndex;
        this.noteTime      = noteTime;
        this.approachTime  = approachTime;
        this.spawnY        = spawnY;
        this.hitY          = hitY;
        this.gm            = gm;
        this.handled       = false;

        rt = GetComponent<RectTransform>();
        // set initial vertical position
        Vector2 pos = rt.anchoredPosition;
        pos.y = spawnY;
        rt.anchoredPosition = pos;
    }

    void Update()
    {
        float now = gm.audioPlayer.time;
        // t goes from 0 (start falling) → 1 (hit line)
        float t = (now - (noteTime - approachTime)) / approachTime;

        if (t >= 1f)
        {
            // if we haven't hit it
            if (!handled)
            {
                gm.RegisterMiss();
                handled = true;
            }
            Destroy(gameObject);
            return;
        }

        if (t >= 0f)
        {
            // move from spawnY down to hitY
            float y = Mathf.Lerp(spawnY, hitY, t);
            Vector2 pos = rt.anchoredPosition;
            pos.y = y;
            rt.anchoredPosition = pos;
        }
    }
}
