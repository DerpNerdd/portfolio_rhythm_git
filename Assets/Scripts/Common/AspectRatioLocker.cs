// Assets/Scripts/AspectRatioLocker.cs
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class AspectRatioLocker : MonoBehaviour
{
    [Tooltip("Width ÷ Height e.g. 16f/9f")]
    public float targetAspect = 16f / 9f;

    void Update()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;
        var cam = GetComponent<Camera>();
        if (scaleHeight < 1f)
        {
            // Add letterbox
            var rect = cam.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1f - scaleHeight) / 2f;
            cam.rect = rect;
        }
        else
        {
            // Add pillarbox
            float scaleWidth = 1f / scaleHeight;
            var rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0;
            cam.rect = rect;
        }
    }
}
