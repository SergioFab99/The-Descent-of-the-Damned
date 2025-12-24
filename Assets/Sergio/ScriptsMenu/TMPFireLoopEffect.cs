using UnityEngine;
using TMPro;

public class TMPFireLoopEffect : MonoBehaviour
{
    private TMP_Text textMesh;
    private TMP_MeshInfo[] originalMesh;

    public float flameHeight = 10f;
    public float flameSpeed = 2f;
    public float distortionStrength = 2f;

    public Color baseColor = new Color(1f, 0.3f, 0f);
    public Color hotColor = new Color(1f, 0.9f, 0.2f);
    public Color darkColor = new Color(0.6f, 0.1f, 0f);

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        textMesh.ForceMeshUpdate();
        originalMesh = textMesh.textInfo.CopyMeshInfoVertexData();
    }

    void Update()
    {
        AnimateFire();
    }

    void AnimateFire()
    {
        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        float time = Time.time * flameSpeed;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)continue;
            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertIndex = textInfo.characterInfo[i].vertexIndex;
            float noise = Mathf.PerlinNoise(i * 0.3f, time);
            float verticalOffset = noise * flameHeight;
            Vector3 flameOffset = new Vector3(Mathf.Sin(time + i) * distortionStrength,verticalOffset,0);
            textInfo.meshInfo[matIndex].vertices[vertIndex + 0] = originalMesh[matIndex].vertices[vertIndex + 0];
            textInfo.meshInfo[matIndex].vertices[vertIndex + 3] = originalMesh[matIndex].vertices[vertIndex + 3];
            textInfo.meshInfo[matIndex].vertices[vertIndex + 1] = originalMesh[matIndex].vertices[vertIndex + 1] + flameOffset;
            textInfo.meshInfo[matIndex].vertices[vertIndex + 2] = originalMesh[matIndex].vertices[vertIndex + 2] + flameOffset;
            Color fireColor = Color.Lerp(darkColor,hotColor,noise);
            fireColor = Color.Lerp(fireColor,baseColor,Mathf.Sin(time + i) * 0.5f + 0.5f);
            textInfo.meshInfo[matIndex].colors32[vertIndex + 0] = fireColor;
            textInfo.meshInfo[matIndex].colors32[vertIndex + 1] = fireColor;
            textInfo.meshInfo[matIndex].colors32[vertIndex + 2] = fireColor;
            textInfo.meshInfo[matIndex].colors32[vertIndex + 3] = fireColor;
        }

        textMesh.UpdateVertexData(
            TMP_VertexDataUpdateFlags.Vertices |
            TMP_VertexDataUpdateFlags.Colors32
        );
    }
}
