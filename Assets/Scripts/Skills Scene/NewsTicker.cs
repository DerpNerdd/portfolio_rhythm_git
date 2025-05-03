using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class BorderTextScroller : MonoBehaviour
{
    [Header("Border & Prefab")]
    [Tooltip("The UI RectTransform defining the inner perimeter")]
    public RectTransform      borderRect;
    [Tooltip("TextMeshProUGUI prefab for each piece")]
    public TextMeshProUGUI    textPrefab;

    [Header("Message Settings")]
    [Tooltip("Words (or segments) in one ‘message set’, e.g. [\"MY\",\"SKILLS\"]")]
    public List<string>       messageParts = new List<string> { "MY", "SKILLS" };
    [Tooltip("How many times to repeat that set around the border")]
    public int                repeatCount  = 1;

    [Header("Spacing (UI units)")]
    [Tooltip("Gap between consecutive parts within a single set")]
    public float              partSeparation = 1.2f;
    [Tooltip("Larger gap between the end of one set and start of the next")]
    public float              setSeparation  = 10f;

    [Header("Scroll Speed")]
    [Tooltip("Units per second along the perimeter")]
    public float              speed = 1f;

    // Internals
    float _perimeter;
    Vector2 _centerOffset;
    struct Part { public TextMeshProUGUI text; public float offset; }
    List<Part> _items = new List<Part>();

    void Start()
    {
        // perimeter of the inside of the border
        float w = borderRect.rect.width;
        float h = borderRect.rect.height;
        _perimeter   = 2f * (w + h);
        _centerOffset = borderRect.anchoredPosition;

        // calculate how far one full set takes, to space repeats
        float singleSetLength = (messageParts.Count - 1) * partSeparation;
        float fullCycleLength = singleSetLength + setSeparation;

        // instantiate all parts
        for (int r = 0; r < repeatCount; r++)
        {
            for (int p = 0; p < messageParts.Count; p++)
            {
                Part part = new Part();
                part.text = Instantiate(textPrefab, transform);
                part.text.text = messageParts[p];
                part.text.rectTransform.pivot = new Vector2(0.5f, 0.5f);

                // offset along the perimeter:
                //   r * fullCycleLength + p * partSeparation
                part.offset = r * fullCycleLength + p * partSeparation;
                _items.Add(part);
            }
        }
    }

    void Update()
    {
        if (_items.Count == 0) return;

        float w = borderRect.rect.width;
        float h = borderRect.rect.height;
        float basePos = Time.time * speed;

        foreach (var part in _items)
        {
            // advance & wrap
            float d = (basePos + part.offset) % _perimeter;

            // figure out which edge it’s on and the local XY + rotation
            Vector2 localPos;
            float   angle;
            if (d < w)
            {
                localPos = new Vector2(-w/2 + d,  h/2);
                angle    = 0f;
            }
            else if (d < w + h)
            {
                float dd = d - w;
                localPos = new Vector2( w/2,  h/2 - dd);
                angle    = -90f;
            }
            else if (d < w + h + w)
            {
                float dd = d - w - h;
                localPos = new Vector2( w/2 - dd, -h/2);
                angle    = 180f;
            }
            else
            {
                float dd = d - w - h - w;
                localPos = new Vector2(-w/2, -h/2 + dd);
                angle    = 90f;
            }

            // apply UI positioning & rotation
            part.text.rectTransform.anchoredPosition = _centerOffset + localPos;
            part.text.rectTransform.localEulerAngles  = new Vector3(0,0,angle);
        }
    }
}
