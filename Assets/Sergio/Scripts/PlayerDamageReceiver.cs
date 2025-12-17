using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerDamageReceiver : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool invulnerable;
    [SerializeField] private float invulnerableUntil;

    public bool IsInvulnerable => invulnerable && Time.time < invulnerableUntil;

    public void SetInvulnerableForSeconds(float seconds)
    {
        if (seconds <= 0f)
        {
            invulnerable = false;
            invulnerableUntil = 0f;
            return;
        }

        invulnerable = true;
        invulnerableUntil = Mathf.Max(invulnerableUntil, Time.time + seconds);
    }

    public bool TryTakeDamage(float amount)
    {
        if (IsInvulnerable)
            return false;

        // No hay sistema de vida/daño aún en el proyecto.
        // Cuando exista, aquí sería el punto único para restar vida y disparar eventos.
        return true;
    }

    private void Update()
    {
        if (invulnerable && Time.time >= invulnerableUntil)
        {
            invulnerable = false;
        }
    }
}
