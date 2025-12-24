using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ShieldPlayer : MonoBehaviour
{
    [Header("Shield")]
    [SerializeField] private KeyCode shieldKey = KeyCode.Q;
    [SerializeField] private float shieldDurationSeconds = 5f;

    [Header("Shield Visual")]
    [SerializeField] private Material shieldMaterial;

    [Header("Enemy Filter")]
    [SerializeField] private string enemyTagLowercase = "enemy";
    [SerializeField] private string enemyTagUppercase = "Enemy";

    private PlayerDamageReceiver damageReceiver;
    private Collider2D[] playerColliders;
    private MeshRenderer[] meshRenderers;
    private Material[] originalMaterials;

    private bool shieldActive;
    private float shieldUntil;

    private struct IgnoredPair
    {
        public Collider2D playerCollider;
        public Collider2D otherCollider;
    }

    private readonly System.Collections.Generic.List<IgnoredPair> ignoredPairs = new System.Collections.Generic.List<IgnoredPair>(32);

    private void CacheRenderers()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>(includeInactive: false);

        if (meshRenderers == null || meshRenderers.Length == 0)
        {
            originalMaterials = null;
            return;
        }

        originalMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            var r = meshRenderers[i];
            originalMaterials[i] = r != null ? r.material : null;
        }
    }

    private void Awake()
    {
        damageReceiver = GetComponent<PlayerDamageReceiver>();
        playerColliders = GetComponentsInChildren<Collider2D>(includeInactive: false);
        CacheRenderers();

        if (damageReceiver == null)
        {
            damageReceiver = gameObject.AddComponent<PlayerDamageReceiver>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(shieldKey))
        {
            ActivateShield(shieldDurationSeconds);
        }

        if (shieldActive && Time.time >= shieldUntil)
        {
            DeactivateShield();
        }
    }

    private void ActivateShield(float durationSeconds)
    {
        if (durationSeconds <= 0f)
            return;

        shieldActive = true;
        shieldUntil = Time.time + durationSeconds;

        if (shieldMaterial != null)
        {
            if (meshRenderers == null || meshRenderers.Length == 0 || originalMaterials == null || originalMaterials.Length != meshRenderers.Length)
                CacheRenderers();

            if (meshRenderers != null)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    var r = meshRenderers[i];
                    if (r == null)
                        continue;
                    r.material = shieldMaterial;
                }
            }
        }

        if (damageReceiver != null)
        {
            damageReceiver.SetInvulnerableForSeconds(durationSeconds);
        }
    }

    private void DeactivateShield()
    {
        shieldActive = false;

        if (meshRenderers == null || originalMaterials == null || (meshRenderers != null && originalMaterials.Length != meshRenderers.Length))
            CacheRenderers();

        if (meshRenderers != null && originalMaterials != null)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                var r = meshRenderers[i];
                if (r == null)
                    continue;
                r.material = originalMaterials[i];
            }
        }

        for (int i = ignoredPairs.Count - 1; i >= 0; i--)
        {
            var pair = ignoredPairs[i];
            if (pair.playerCollider == null || pair.otherCollider == null)
            {
                ignoredPairs.RemoveAt(i);
                continue;
            }

            Physics2D.IgnoreCollision(pair.playerCollider, pair.otherCollider, false);
            ignoredPairs.RemoveAt(i);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!shieldActive)
            return;

        TryIgnoreEnemyCollider(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!shieldActive)
            return;

        TryIgnoreEnemyCollider(other);
    }

    private void TryIgnoreEnemyCollider(Collider2D other)
    {
        if (other == null)
            return;

        if (!IsEnemy(other.gameObject))
            return;

        if (playerColliders == null || playerColliders.Length == 0)
            playerColliders = GetComponentsInChildren<Collider2D>(includeInactive: false);

        for (int i = 0; i < playerColliders.Length; i++)
        {
            var playerCol = playerColliders[i];
            if (playerCol == null)
                continue;

            Physics2D.IgnoreCollision(playerCol, other, true);
            ignoredPairs.Add(new IgnoredPair { playerCollider = playerCol, otherCollider = other });
        }
    }

    private bool IsEnemy(GameObject obj)
    {
        if (obj == null)
            return false;

        string t = obj.tag;
        return t == enemyTagLowercase || t == enemyTagUppercase;
    }
}
