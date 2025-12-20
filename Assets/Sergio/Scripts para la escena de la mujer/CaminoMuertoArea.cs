using UnityEngine;

public class CaminoMuertoArea : MonoBehaviour
{
    public float duration = 5f;
    public float damagePerSecond = 10f;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Aquí iría un sistema de vida real
            Debug.Log("Daño al enemigo: " + damagePerSecond * Time.deltaTime);
        }
    }
}
