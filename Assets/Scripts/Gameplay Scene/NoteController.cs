// Assets/Scripts/Gameplay Scene/NoteController.cs
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class NoteController : MonoBehaviour
{
    public int    laneIndex { get; private set; }
    public float  noteTime  { get; private set; }
    public bool   handled   { get; set; }

    private float spawnY;
    private float hitY;
    private float approachTime;
    private float fallSpeed;
    private float offscreenY;
    private RhythmGameManager gm;
    private RectTransform rt;

    /// <summary>
    /// Called by RhythmGameManager.Init after spawning.
    /// </summary>
    public void Init(int laneIndex,
                     float noteTime,
                     float approachTime,
                     float spawnY,
                     float hitY,
                     RhythmGameManager gm)
    {
        this.laneIndex    = laneIndex;
        this.noteTime     = noteTime;
        this.approachTime = approachTime;
        this.spawnY       = spawnY;
        this.hitY         = hitY;
        this.gm           = gm;
        this.handled      = false;

        rt = GetComponent<RectTransform>();

        // compute steady fall speed for after the hit moment
        fallSpeed = (spawnY - hitY) / approachTime;

        // position note just above the container so it's invisible initially
        offscreenY = spawnY + rt.rect.height;
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, offscreenY);
    }

    void Update()
    {
        float now       = gm.audioPlayer.time;
        float startTime = noteTime - approachTime;

        // not time to show it yet
        if (now < startTime)
            return;

        Vector2 pos = rt.anchoredPosition;

        if (now < noteTime)
        {
            // phase 1: lerp from spawnY → hitY
            float t = (now - startTime) / approachTime;
            pos.y = Mathf.Lerp(spawnY, hitY, t);
        }
        else
        {
            // phase 2: continue falling at constant speed
            pos.y -= fallSpeed * Time.deltaTime;

            // register a miss the first frame past hitTime
            if (!handled)
            {
                gm.RegisterMiss();
                handled = true;
            }
        }

        rt.anchoredPosition = pos;

        // destroy once fully off the bottom
        if (pos.y < 0f)
            Destroy(gameObject);
    }
}
