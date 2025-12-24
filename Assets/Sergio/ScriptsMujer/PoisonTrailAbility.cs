using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoisonTrailAbility : AbilityBase
{
    [SerializeField] private float spawnInterval = 0.15f;
    [SerializeField] private float trailSize = 0.5f;
    [SerializeField] private Color trailColor = Color.green;

    private bool isActive;
    private float timer;

    public override void Activate()
    {
        isActive = !isActive;
    }

    private void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTrail();
            timer = 0f;
        }
    }

    private void SpawnTrail()
    {
        GameObject trail = GameObject.CreatePrimitive(PrimitiveType.Quad);
        trail.transform.position = GetGroundPosition();
        trail.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        trail.transform.localScale = Vector3.one * trailSize;

        Renderer r = trail.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Unlit/Color"));
        r.material.color = trailColor;

        Destroy(trail.GetComponent<Collider>());
        Destroy(trail, 5f);
    }

    private Vector3 GetGroundPosition()
    {
        Vector3 pos = transform.position;
        pos.y = 0.01f;
        return pos;
    }
}
