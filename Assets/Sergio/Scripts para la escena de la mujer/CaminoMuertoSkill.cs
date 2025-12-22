using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CaminoMuertoSkill : SkillBase
{
    public GameObject areaPrefab;
    
    [Header("Configuración del Área")]
    public float areaRadius = 3f;
    public float areaDuration = 5f;
    
    [Header("Colores para Gizmos")]
    public Color gizmoColor = new Color(1f, 0f, 0f, 0.3f);
    public Color wireColor = Color.red;
    
    [Header("Visualización en Editor")]
    public bool showGizmos = true;
    public bool showWireframe = true;

    protected override void Activate(GameObject caster)
    {
        GameObject area = Object.Instantiate(
            areaPrefab,
            caster.transform.position,
            Quaternion.identity
        );

        CaminoMuertoArea areaScript = area.GetComponent<CaminoMuertoArea>();
        if (areaScript != null)
        {
            areaScript.duration = areaDuration;
        }

        SphereCollider collider = area.GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.radius = areaRadius;
        }
    }

#if UNITY_EDITOR
    
    public void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        GameObject selected = Selection.activeGameObject;
        
        if (selected != null && selected.GetComponent<PlayerSkillController>() != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(selected.transform.position, areaRadius);
            
            if (showWireframe)
            {
                Gizmos.color = wireColor;
                Gizmos.DrawWireSphere(selected.transform.position, areaRadius);
            }
            
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 12;
            style.fontStyle = FontStyle.Bold;
            Handles.Label(selected.transform.position + Vector3.up * (areaRadius + 0.5f), 
                         $"Área Camino Muerto\nRadio: {areaRadius}\nDuración: {areaDuration}s", style);
        }
    }

    public void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }
#endif
}