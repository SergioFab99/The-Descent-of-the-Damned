using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GenerateSwordsAround : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode activationKey = KeyCode.X;
    [SerializeField] private bool toggleOnKey = true;

    [Header("Prefabs")]
    [Tooltip("Si 'Sword Prefabs' tiene 4 elementos, se usan esos. Si no, se repite 'Sword Prefab' 4 veces.")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private GameObject[] swordPrefabs;

    [Header("Spawn")]
    [Min(0f)]
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float heightOffset = 0f;

    [Header("Rotation")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeedDegreesPerSecond = 360f;

    [Header("Projectile")]
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float projectileLifeSeconds = 6f;
    [SerializeField] private float projectileSpeedMultiplier = 2f;

    private Transform pivot;
    private readonly List<GameObject> spawned = new List<GameObject>(4);
    private bool isActive;

    private void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            if (!isActive)
            {
                Activate();
            }
            else if (spawned.Count > 0)
            {
                FireOne();
            }
            else if (toggleOnKey)
            {
                Deactivate();
            }
        }

        if (!isActive || pivot == null)
        {
            return;
        }

        var axis = rotationAxis.sqrMagnitude > 0.0001f ? rotationAxis.normalized : Vector3.up;
        pivot.Rotate(axis, rotationSpeedDegreesPerSecond * Time.deltaTime, Space.Self);
    }

    private void Activate()
    {
        if (!HasAnyPrefab())
        {
            Debug.LogWarning("GenerateSwordsAround: Asigna 'Sword Prefab' o 4 elementos en 'Sword Prefabs' en el Inspector.", this);
            return;
        }

        EnsurePivot();
        SpawnSwords();
        isActive = true;
    }

    private void Deactivate()
    {
        isActive = false;
        CleanupSpawned();

        if (pivot != null)
        {
            Destroy(pivot.gameObject);
            pivot = null;
        }
    }

    private void EnsurePivot()
    {
        if (pivot != null)
        {
            return;
        }

        var pivotGO = new GameObject("SwordsPivot");
        pivot = pivotGO.transform;
        pivot.SetParent(transform);
        pivot.localPosition = new Vector3(0f, heightOffset, 0f);
        pivot.localRotation = Quaternion.identity;
    }

    private void SpawnSwords()
    {
        CleanupSpawned();

        var offsets = new Vector3[4]
        {
            new Vector3(radius, 0f, 0f),
            new Vector3(-radius, 0f, 0f),
            new Vector3(0f, 0f, radius),
            new Vector3(0f, 0f, -radius),
        };

        for (int i = 0; i < 4; i++)
        {
            var prefab = GetPrefabForIndex(i);
            if (prefab == null)
            {
                continue;
            }

            var instance = Instantiate(prefab, pivot);
            instance.transform.localPosition = offsets[i];
            instance.transform.localRotation = Quaternion.identity;
            spawned.Add(instance);
        }
    }

    private GameObject GetPrefabForIndex(int index)
    {
        if (swordPrefabs != null && swordPrefabs.Length > 0)
        {
            if (index < swordPrefabs.Length && swordPrefabs[index] != null)
            {
                return swordPrefabs[index];
            }

            for (int i = swordPrefabs.Length - 1; i >= 0; i--)
            {
                if (swordPrefabs[i] != null)
                {
                    return swordPrefabs[i];
                }
            }
        }

        return swordPrefab;
    }

    private bool HasAnyPrefab()
    {
        if (swordPrefab != null)
        {
            return true;
        }

        if (swordPrefabs == null)
        {
            return false;
        }

        for (int i = 0; i < swordPrefabs.Length; i++)
        {
            if (swordPrefabs[i] != null)
            {
                return true;
            }
        }

        return false;
    }

    private void CleanupSpawned()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            if (spawned[i] != null)
            {
                Destroy(spawned[i]);
            }
        }

        spawned.Clear();
    }

    private void OnDisable()
    {
        if (isActive)
        {
            Deactivate();
        }
    }

    private void FireAll()
    {
        FireOne();
    }

    private void FireOne()
    {
        if (spawned.Count == 0)
        {
            return;
        }

        var sword = spawned[0];
        spawned.RemoveAt(0);

        if (sword != null)
        {
            sword.transform.SetParent(null);

            var rb = sword.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = sword.AddComponent<Rigidbody>();
            }

            float speed = Mathf.Max(0f, projectileSpeed * projectileSpeedMultiplier);
            rb.linearVelocity = GetHorizontalForward() * speed;

            if (projectileLifeSeconds > 0f)
            {
                Destroy(sword, projectileLifeSeconds);
            }
        }

        if (spawned.Count == 0)
        {
            isActive = false;

            if (pivot != null)
            {
                Destroy(pivot.gameObject);
                pivot = null;
            }
        }
    }

    private Vector3 GetHorizontalForward()
    {
        Vector3 forward;

        if (Camera.main != null)
        {
            forward = Camera.main.transform.forward;
        }
        else
        {
            forward = transform.forward;
        }

        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f)
        {
            forward = Vector3.forward;
        }

        return forward.normalized;
    }
}
