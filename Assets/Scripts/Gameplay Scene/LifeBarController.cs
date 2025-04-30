// LifeBarController.cs
using UnityEngine;
using UnityEngine.UI;

public class LifeBarController : MonoBehaviour
{
    [Header("UI References")]
    public Image fillImage;                // assign LifeBarFill here

    [Header("Life Settings")]
    [Range(0,1)] public float life = 1f;  // current life (0–1)
    public float gainPerHit  = 0.05f;      // how much to add per note
    public float lossPerMiss = 0.10f;      // how much to drop per miss
    public float colorCycleSpeed = 0.5f;   // how fast the rainbow cycles

    RhythmGameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<RhythmGameManager>();
    }

    void Start()
    {
        UpdateFill();
    }

    void Update()
    {
        // cycle through rainbow
        float hue = (Time.time * colorCycleSpeed) % 1f;
        fillImage.color = Color.HSVToRGB(hue, 1f, 1f);
    }

    public void OnNoteHit()
    {
        ChangeLife(  Mathf.Abs(gainPerHit) );
    }

    public void OnNoteMiss()
    {
        ChangeLife( -Mathf.Abs(lossPerMiss) );
    }

    void ChangeLife(float delta)
    {
        life = Mathf.Clamp01(life + delta);
        UpdateFill();

        if (life <= 0f)
            gameManager.ShowResults();
    }

    void UpdateFill()
    {
        fillImage.fillAmount = life;
    }
}
