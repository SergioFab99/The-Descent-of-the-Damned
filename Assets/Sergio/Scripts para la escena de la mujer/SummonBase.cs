using UnityEngine;

public abstract class SummonBase : MonoBehaviour
{
    public float lifeTime = 20f;
    protected GameObject owner;

    public void Initialize(GameObject owner)
    {
        this.owner = owner;
        Invoke(nameof(Die), lifeTime);
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
