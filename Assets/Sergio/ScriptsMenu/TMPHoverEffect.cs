using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TMPCrackHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text textMesh;
    private TMP_MeshInfo[] originalMesh;

    public float crackStrength = 8f;
    public float jitterStrength = 1.5f;
    public float scaleAmount = 1.05f;
    public float animationSpeed = 10f;

    [Header("Fire Colors")]
    public Color hotColor = new Color(1f, 0.9f, 0.2f);
    public Color midColor = new Color(1f, 0.4f, 0f);
    public Color burnColor = new Color(0.15f, 0.05f, 0f);

    [Range(0f, 1f)]
    public float burnAmount = 0.4f;

    private bool isHovering;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        textMesh.ForceMeshUpdate();
        originalMesh = textMesh.textInfo.CopyMeshInfoVertexData();
    }

    void Update()
    {
        if (isHovering)
            ApplyCrackAndFireEffect();
        else
            RestoreText();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        transform.localScale = Vector3.one * scaleAmount;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        transform.localScale = Vector3.one;
    }

    void ApplyCrackAndFireEffect()
    {
        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        float time = Time.time * 3f;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertIndex = textInfo.characterInfo[i].vertexIndex;

            float dir = Random.Range(-1f, 1f);

                Vector3 topOffset = new Vector3(
                Random.Range(-jitterStrength, jitterStrength),
                crackStrength * dir,
                0);

                Vector3 bottomOffset = new Vector3(
                Random.Range(-jitterStrength, jitterStrength),
                -crackStrength * dir,
                0);

                textInfo.meshInfo[matIndex].vertices[vertIndex + 0] =
                originalMesh[matIndex].vertices[vertIndex + 0] + bottomOffset;
                textInfo.meshInfo[matIndex].vertices[vertIndex + 3] =
                originalMesh[matIndex].vertices[vertIndex + 3] + bottomOffset;

                textInfo.meshInfo[matIndex].vertices[vertIndex + 1] =
                originalMesh[matIndex].vertices[vertIndex + 1] + topOffset;
                textInfo.meshInfo[matIndex].vertices[vertIndex + 2] =
                originalMesh[matIndex].vertices[vertIndex + 2] + topOffset;

                float noise = Mathf.PerlinNoise(i * 0.4f, time);

                Color fireColor = Color.Lerp(midColor, hotColor, noise);
                fireColor = Color.Lerp(fireColor, burnColor, burnAmount + Random.Range(0f, 0.2f));

                for (int j = 0; j < 4; j++)
                textInfo.meshInfo[matIndex].colors32[vertIndex + j] = fireColor;
        }

            textMesh.UpdateVertexData(
            TMP_VertexDataUpdateFlags.Vertices |
            TMP_VertexDataUpdateFlags.Colors32
        );
    }

    void RestoreText()
    {
        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertIndex = textInfo.characterInfo[i].vertexIndex;

            for (int j = 0; j < 4; j++)
            {
                Vector3 current = textInfo.meshInfo[matIndex].vertices[vertIndex + j];
                Vector3 target = originalMesh[matIndex].vertices[vertIndex + j];
                textInfo.meshInfo[matIndex].vertices[vertIndex + j] =
                Vector3.Lerp(current, target, Time.deltaTime * animationSpeed);
                Color currentColor = textInfo.meshInfo[matIndex].colors32[vertIndex + j];
                Color targetColor = originalMesh[matIndex].colors32[vertIndex + j];
                textInfo.meshInfo[matIndex].colors32[vertIndex + j] =
                    Color.Lerp(currentColor, targetColor, Time.deltaTime * animationSpeed);
            }
        }

        textMesh.UpdateVertexData(
            TMP_VertexDataUpdateFlags.Vertices |
            TMP_VertexDataUpdateFlags.Colors32
        );
    }
}
