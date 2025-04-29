// Assets/Scripts/Gameplay Scene/HitLineManager.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates hitline arrow sprites based on GameSettings keybinds.
/// </summary>
public class HitLineManager : MonoBehaviour
{
    [Header("Arrow Images (in order)")]
    [Tooltip("Drag your 4 hitline arrow Image components here (0=left,1=down,2=up,3=right)")]
    public Image[] arrowImages;          // length must be 4

    [Header("Sprites")]
    [Tooltip("Normal (unpressed) arrow sprites")]        
    public Sprite[] normalSprites;       // length must be 4
    [Tooltip("Pressed-down arrow sprites")]  
    public Sprite[] pressedSprites;      // length must be 4

    void Awake()
    {
        if (arrowImages.Length != 4 || normalSprites.Length != 4 || pressedSprites.Length != 4)
        {
            Debug.LogError("HitLineManager: You must assign exactly 4 images and 4 sprites per array.");
            enabled = false;
        }
    }

    void Update()
    {
        var keys = GameSettings.LaneKeys;
        int count = Mathf.Min(keys.Length, arrowImages.Length);
        for (int i = 0; i < count; i++)
        {
            if (Input.GetKeyDown(keys[i]))
                arrowImages[i].sprite = pressedSprites[i];
            if (Input.GetKeyUp(keys[i]))
                arrowImages[i].sprite = normalSprites[i];
        }
    }
}
