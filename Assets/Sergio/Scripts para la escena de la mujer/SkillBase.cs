using UnityEngine;

public abstract class SkillBase
{
    public float cooldown;
    protected float lastUseTime;

    public bool CanUse()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    public void TryUse(GameObject caster)
    {
        if (!CanUse()) return;

        lastUseTime = Time.time;
        Activate(caster);
    }

    protected abstract void Activate(GameObject caster);
}
