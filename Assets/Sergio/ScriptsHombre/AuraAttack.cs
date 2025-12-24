using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AuraAttack : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode fireKey = KeyCode.R;

    [Header("Timing")]
    [SerializeField] private float durationSeconds = 1.5f;
    [SerializeField] private float cooldownSeconds = 4f;
    [SerializeField] private float tickIntervalSeconds = 0.25f;

    [Header("Area")]
    [SerializeField] private float radius = 2.5f;
    [SerializeField] private Vector3 centerOffset = new Vector3(0f, 1f, 0f);

    [Header("Enemy Filter")]
    [SerializeField] private string enemyTagLowercase = "enemy";
    [SerializeField] private string enemyTagUppercase = "Enemy";

    [Header("Burn / Damage (Optional)")]
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private bool sendDamageMessages = true;
    [SerializeField] private string[] damageMessageNames = new string[] { "TryTakeDamage", "TakeDamage" };

    [Header("VFX (Optional)")]
    [Tooltip("Si lo asignas, se hace Play/Stop mientras el aura está activa.")]
    [SerializeField] private ParticleSystem auraParticles;
    [Tooltip("Prefab opcional (ParticleSystem/Luz/FX). Se instancia como hijo del jugador mientras dura el aura.")]
    [SerializeField] private GameObject auraVfxPrefab;
    [SerializeField] private Vector3 vfxLocalOffset = Vector3.zero;
    [SerializeField] private Vector3 vfxLocalEulerAngles = Vector3.zero;

    [Header("Debug")]
    [SerializeField] private bool auraActive;
    [SerializeField] private float auraUntil;
    [SerializeField] private float nextReadyTime;

    private Coroutine auraRoutine;
    private GameObject vfxInstance;
    private readonly HashSet<int> uniqueTargetsThisTick = new HashSet<int>(64);
    private Transform cameraTransform;

    public bool IsActive => auraActive;
    public float CooldownRemaining => Mathf.Max(0f, nextReadyTime - Time.time);

    private void Start()
    {
        cameraTransform = Camera.main?.transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(fireKey))
        {
            TryActivate();
        }

        if (auraActive)
        {
            UpdateVfxRotation();
        }
    }

    private void TryActivate()
    {
        if (auraActive)
            return;

        if (Time.time < nextReadyTime)
            return;

        if (durationSeconds <= 0f)
            return;

        auraActive = true;
        auraUntil = Time.time + durationSeconds;
        nextReadyTime = Time.time + Mathf.Max(0f, cooldownSeconds);

        StartVfx();

        if (auraRoutine != null)
        {
            StopCoroutine(auraRoutine);
        }

        auraRoutine = StartCoroutine(AuraRoutine());
    }

    private IEnumerator AuraRoutine()
    {
        float wait = Mathf.Max(0.01f, tickIntervalSeconds);

        while (Time.time < auraUntil)
        {
            ApplyBurnTick();
            yield return new WaitForSeconds(wait);
        }

        StopVfx();
        auraActive = false;
        auraRoutine = null;
    }

    private void ApplyBurnTick()
    {
        if (radius <= 0f)
            return;

        Vector3 center = transform.position + centerOffset;
        uniqueTargetsThisTick.Clear();

        Collider[] hits3D = Physics.OverlapSphere(center, radius);
        for (int i = 0; i < hits3D.Length; i++)
        {
            var col = hits3D[i];
            if (col == null)
                continue;

            GameObject obj = col.attachedRigidbody != null ? col.attachedRigidbody.gameObject : col.gameObject;
            TryAffectTarget(obj);
        }

        
        Collider2D[] hits2D = Physics2D.OverlapCircleAll(center, radius);
        for (int i = 0; i < hits2D.Length; i++)
        {
            var col = hits2D[i];
            if (col == null)
                continue;

            TryAffectTarget(col.gameObject);
        }
    }

    private void TryAffectTarget(GameObject obj)
    {
        if (obj == null)
            return;

        if (!IsEnemy(obj))
            return;

        int id = obj.GetInstanceID();
        if (!uniqueTargetsThisTick.Add(id))
            return;

        ApplyDamageMessages(obj, damagePerTick);
    }

    private void ApplyDamageMessages(GameObject target, float amount)
    {
        if (!sendDamageMessages)
            return;

        if (target == null)
            return;

        if (amount <= 0f)
            return;

        if (damageMessageNames == null || damageMessageNames.Length == 0)
            return;

        for (int i = 0; i < damageMessageNames.Length; i++)
        {
            string msg = damageMessageNames[i];
            if (string.IsNullOrWhiteSpace(msg))
                continue;

            target.SendMessage(msg, amount, SendMessageOptions.DontRequireReceiver);
        }
    }

    private bool IsEnemy(GameObject obj)
    {
        if (obj == null)
            return false;

        string t = obj.tag;
        return t == enemyTagLowercase || t == enemyTagUppercase;
    }

    private void StartVfx()
    {
        if (auraParticles != null)
        {
            auraParticles.Play();
        }

        if (auraVfxPrefab != null && vfxInstance == null)
        {
            vfxInstance = Instantiate(auraVfxPrefab, transform);
            vfxInstance.transform.localPosition = vfxLocalOffset;
            UpdateVfxRotation();
        }
    }

    private void UpdateVfxRotation()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform == null)
                return;
        }

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        
        if (cameraForward.sqrMagnitude < 0.001f)
        {
            cameraForward = transform.forward;
            cameraForward.y = 0f;
        }
        
        cameraForward.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);

        if (vfxInstance != null)
        {
            vfxInstance.transform.rotation = targetRotation * Quaternion.Euler(vfxLocalEulerAngles);
        }

        if (auraParticles != null)
        {
            auraParticles.transform.rotation = targetRotation * Quaternion.Euler(vfxLocalEulerAngles);
        }
    }

    private void StopVfx()
    {
        if (auraParticles != null)
        {
            auraParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (vfxInstance != null)
        {
            Destroy(vfxInstance);
            vfxInstance = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (radius <= 0f)
            return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position + centerOffset, radius);
    }
}
