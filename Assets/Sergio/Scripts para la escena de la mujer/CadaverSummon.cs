using UnityEngine;

public class CadaverSummon : SummonBase
{
    public float followDistance = 2f;

    void Update()
    {
        if (owner == null) return;

        Vector3 targetPos = owner.transform.position;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            Time.deltaTime * 3f
        );
    }
}
