using UnityEngine;

public class MaskRotator : MonoBehaviour
{
    // Rotation speed in radians per second.
    public float rotationSpeed = 0.1f;
    
    // Current rotation value in radians.
    private float currentRotation = 0f;
    
    // Cached Renderer component.
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("MaskRotator: No Renderer found on " + gameObject.name);
        }
    }

    void Update()
    {
        // Increment rotation. (Use a negative value for opposite direction if needed.)
        currentRotation += rotationSpeed * Time.deltaTime;
        
        // Update the shader property _MaskRotation.
        if (rend != null)
        {
            rend.material.SetFloat("_MaskRotation", currentRotation);
        }
    }
}
