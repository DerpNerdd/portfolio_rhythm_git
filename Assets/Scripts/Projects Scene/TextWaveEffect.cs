using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextWaveEffect : MonoBehaviour
{
    [Tooltip("How tall the wave is (in local units).")]
    public float amplitude = 5f;

    [Tooltip("Length (in local units) of one full wave cycle.")]
    public float waveLength = 50f;

    [Tooltip("Speed at which the wave scrolls.")]
    public float speed = 1f;

    TMP_Text       textMesh;
    TMP_TextInfo   textInfo;
    TMP_MeshInfo[] cachedMeshInfo;
    bool           textChanged;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        textMesh.ForceMeshUpdate();
        textInfo = textMesh.textInfo;
        // cache the original vertex positions
        cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
    }

    void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    void OnTextChanged(Object obj)
    {
        if (obj == textMesh)
            textChanged = true;
    }

    void Update()
    {
        // if someone changed the text at runtime, re-cache verts
        if (textChanged)
        {
            textMesh.ForceMeshUpdate();
            textInfo       = textMesh.textInfo;
            cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
            textChanged    = false;
        }

        float time = Time.time * speed;

        // for each sub-mesh/material…
        for (int m = 0; m < textInfo.meshInfo.Length; m++)
        {
            var meshInfo = textInfo.meshInfo[m];
            var verts    = meshInfo.vertices;
            var orig     = cachedMeshInfo[m].vertices;

            // for each character…
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible || charInfo.materialReferenceIndex != m)
                    continue;

                int vi = charInfo.vertexIndex;

                // compute the horizontal center of this glyph
                float xMid = (orig[vi + 0].x + orig[vi + 2].x) * 0.5f;

                // wave offset = sin( xMid/waveLength * 2π + time ) * amplitude
                float wave = Mathf.Sin((xMid / waveLength * Mathf.PI * 2f) + time) * amplitude;
                Vector3 offset = new Vector3(0, wave, 0);

                // apply it to all four verts of this char
                verts[vi + 0] = orig[vi + 0] + offset;
                verts[vi + 1] = orig[vi + 1] + offset;
                verts[vi + 2] = orig[vi + 2] + offset;
                verts[vi + 3] = orig[vi + 3] + offset;
            }

            // push it back and redraw
            var mesh = meshInfo.mesh;
            mesh.vertices = verts;
            textMesh.UpdateGeometry(mesh, m);
        }
    }
}
