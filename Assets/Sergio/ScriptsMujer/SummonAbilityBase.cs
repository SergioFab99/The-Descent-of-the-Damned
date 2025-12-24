using UnityEngine;
using System.Collections;

public abstract class SummonAbilityBase : AbilityBase
{
    public GameObject summonPrefab;
    public int summonCount = 1;
    public float summonRadius = 2f;
    public LayerMask groundLayer;

    protected void Summon()
    {
        if (!summonPrefab) return;

        float angleStep = 360f / summonCount;
        Vector3 center = transform.position;

        for (int i = 0; i < summonCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f,Mathf.Sin(angle)) * summonRadius;
            Vector3 rayOrigin = center + offset + Vector3.up * 10f;
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 50f, groundLayer))
            {
                StartCoroutine(SpawnSafely(hit.point));
            }
        }
    }

    private IEnumerator SpawnSafely(Vector3 groundPoint)
    {
        GameObject obj = Instantiate(summonPrefab,groundPoint + Vector3.up * 3f,Quaternion.Euler(-90f, 0f, 0f));
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        Collider col = obj.GetComponentInChildren<Collider>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        yield return null;
        if (col)
        {
            obj.transform.position = groundPoint + Vector3.up * col.bounds.extents.y;
        }
        else
        {
            obj.transform.position = groundPoint;
        }

        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Destroy(obj, 10f);
    }
}
