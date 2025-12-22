using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CaminoMuertoArea : MonoBehaviour
{
    public float duration = 5f;
    public float damagePerSecond = 10f;
    public float tickRate = 0.5f;
    public float areaRadius = 3f;
    private float nextTickTime;
    
    [Header("Colores Gizmos")]
    public Color areaColor = new Color(1f, 0.5f, 0f, 0.2f);
    public Color pulseColor = Color.yellow;

    void Start()
    {
        Destroy(gameObject, duration);
        
        SphereCollider collider = GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
            areaRadius = collider.radius;
        }
    }

    void Update()
    {
    }

    void OnTriggerStay(Collider other)
    {
        if (Time.time >= nextTickTime)
        {
            if (other.CompareTag("Enemy"))
            {
                Debug.Log($"Daño a {other.name}: {damagePerSecond * tickRate}");
                nextTickTime = Time.time + tickRate;
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = areaColor;
        Gizmos.DrawSphere(transform.position, areaRadius);
        
        Gizmos.color = pulseColor;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
        
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 10;
        Handles.Label(transform.position + Vector3.up * (areaRadius + 0.3f), 
                     $"Área Activa\nTiempo restante: {duration:F1}s", style);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, areaRadius + 0.1f);
    }
#endif
}