using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Graphic))]
public class ParallelogramEffect : BaseMeshEffect
{
    [SerializeField] 
    private float skewAmount = 50f; // Adjust this value in pixels; try different values for a perfect look

    public float SkewAmount
    {
        get { return skewAmount; }
        set { skewAmount = value; graphic.SetVerticesDirty(); }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;
        
        List<UIVertex> verts = new List<UIVertex>();
        vh.GetUIVertexStream(verts);
        if (verts.Count == 0)
            return;

        // Determine the min and max y values among the vertices.
        float minY = verts[0].position.y;
        float maxY = verts[0].position.y;
        for (int i = 1; i < verts.Count; i++)
        {
            float y = verts[i].position.y;
            if (y < minY)
                minY = y;
            if (y > maxY)
                maxY = y;
        }
        float height = maxY - minY;
        if (Mathf.Approximately(height, 0))
            height = 1f; // Prevent division by zero
        
        // Adjust each vertex: shift x based on its normalized y position.
        for (int i = 0; i < verts.Count; i++)
        {
            UIVertex vt = verts[i];
            // Calculate t from 0 at the bottom to 1 at the top.
            float t = (vt.position.y - minY) / height;
            vt.position.x += skewAmount * t;
            verts[i] = vt;
        }
        
        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
    }
}
