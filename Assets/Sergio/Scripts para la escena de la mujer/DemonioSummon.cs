using UnityEngine;

public class DemonioSummon : SummonBase
{
    public float moveSpeed = 2f;

    void Update()
    {
        if (owner == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            owner.transform.position,
            Time.deltaTime * moveSpeed
        );
    }
}
