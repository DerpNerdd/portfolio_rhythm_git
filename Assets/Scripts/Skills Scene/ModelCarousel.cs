using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 3D Model carousel: arranges specified models (or all children) around a circle and spins them with UI buttons.
/// Models in "preserveRotationList" keep their original localRotation and can have a custom Y offset.
/// All other models auto-face the carousel pivot.
/// </summary>
public class ModelCarousel : MonoBehaviour
{
    [Header("Carousel Settings")]
    [Tooltip("Empty GameObject that holds your model instances as children.")]
    public Transform carouselPivot;

    [Tooltip("If non-empty, only these Transforms will be arranged & spun; otherwise all children of the pivot are used.")]
    public List<Transform> models;

    [Tooltip("Transforms to preserve their original rotation (e.g., rotated -90°).")]
    public List<Transform> preserveRotationList;

    [Tooltip("Transforms to preserve all transform values; not repositioned nor rotated.")]
    public List<Transform> preserveAllList;
    
    [Tooltip("Horizontal offset for each preserved 'preserveAllList' model.")]
    public List<float> preserveAllXOffsetList;
    
    [Tooltip("Vertical offset for each preserved 'preserveAllList' model.")]
    public List<float> preserveAllYOffsetList;
    
    [Tooltip("Vertical offset (Y) for each preserved model, in the same order as preserveRotationList.")]
    public List<float> preserveYOffsetList;

    [Tooltip("Radius of the circle on which to place models.")]
    public float radius = 3f;

    [Tooltip("How long the spin animation takes (in seconds).")]
    public float spinDuration = 0.5f;

    [Header("UI Buttons")]
    public Button leftButton;
    public Button rightButton;

    // --- NEW MEMBERS ---
    /// <summary> Index of the currently-selected item (0 = first in list). </summary>
    public int currentIndex { get; private set; } = 0;

    /// <summary> Fired whenever the selection changes; passes the new index. </summary>
    public event Action<int> OnSelectionChanged;
    // -------------------

    // internal state
    List<Transform> items = new List<Transform>();
    Dictionary<Transform, Quaternion> baseRotations = new Dictionary<Transform, Quaternion>();
    int   itemCount;
    float anglePerItem;
    bool  isSpinning;

    void Start()
    {
        if (carouselPivot == null)
        {
            Debug.LogError("ModelCarousel: Please assign a carouselPivot.");
            enabled = false;
            return;
        }

        // Build items list: explicit models or all children
        items.Clear();
        if (models != null && models.Count > 0)
        {
            foreach (var m in models)
                if (m != null)
                    items.Add(m);
        }
        else
        {
            foreach (Transform child in carouselPivot)
                items.Add(child);
        }

        itemCount = items.Count;
        if (itemCount == 0) return;
        anglePerItem = 360f / itemCount;

        // Ensure preserveYOffsetList matches preserveRotationList
        int preserveCount = (preserveRotationList != null) ? preserveRotationList.Count : 0;
        if (preserveYOffsetList == null || preserveYOffsetList.Count != preserveCount)
            preserveYOffsetList = new List<float>(new float[preserveCount]);

        // Cache each preserved model's original rotation
        baseRotations.Clear();
        if (preserveRotationList != null)
        {
            for (int i = 0; i < preserveRotationList.Count; i++)
            {
                Transform t = preserveRotationList[i];
                if (t != null && !baseRotations.ContainsKey(t))
                    baseRotations[t] = t.localRotation;
            }
        }

        // Position & rotate each item
        for (int i = 0; i < itemCount; i++)
        {
            Transform t = items[i];
            t.SetParent(carouselPivot, false);

            float rad = Mathf.Deg2Rad * (anglePerItem * i);
            Vector3 pos = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * radius;

            if (preserveRotationList != null && preserveRotationList.Contains(t))
            {
                int idx = preserveRotationList.IndexOf(t);
                pos.y += preserveYOffsetList[idx];
            }

            if (preserveAllList != null && preserveAllList.Contains(t))
            {
                int idx2 = preserveAllList.IndexOf(t);
                if (preserveAllXOffsetList != null && idx2 < preserveAllXOffsetList.Count)
                    pos.x += preserveAllXOffsetList[idx2];
                if (preserveAllYOffsetList != null && idx2 < preserveAllYOffsetList.Count)
                    pos.y += preserveAllYOffsetList[idx2];
            }

            t.localPosition = pos;

            if (baseRotations.ContainsKey(t))
                t.localRotation = baseRotations[t];
            else
                t.LookAt(carouselPivot.position, Vector3.up);
        }

        // Hook up buttons
        if (leftButton  != null) leftButton .onClick.AddListener(() => Spin(-1));
        if (rightButton != null) rightButton.onClick.AddListener(() => Spin(+1));

        // Fire initial selection event so UI can sync immediately
        OnSelectionChanged?.Invoke(currentIndex);
    }

    /// <summary>
    /// Rotate the carousel by one slot (direction +1 or -1).
    /// </summary>
    public void Spin(int direction)
    {
        if (isSpinning || itemCount == 0) return;
        StartCoroutine(SpinRoutine(direction * anglePerItem));
    }

    IEnumerator SpinRoutine(float deltaAngle)
    {
        isSpinning = true;
        float elapsed = 0f;
        Quaternion start = carouselPivot.localRotation;
        Quaternion end   = start * Quaternion.Euler(0f, deltaAngle, 0f);

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / spinDuration);
            carouselPivot.localRotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }

        carouselPivot.localRotation = end;

        // --- UPDATE INDEX & NOTIFY ---
        int step = Mathf.RoundToInt(deltaAngle / anglePerItem);
        currentIndex = (currentIndex + step + itemCount) % itemCount;
        OnSelectionChanged?.Invoke(currentIndex);
        // -----------------------------

        isSpinning = false;
    }
}
