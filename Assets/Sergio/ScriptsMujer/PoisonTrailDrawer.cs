using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PoisonTrailDrawer : MonoBehaviour
{
    public Transform player;
    public LayerMask groundLayer;

    public float pointSpacing = 0.2f;
    public float groundOffset = 0.02f; 

    private LineRenderer line;
    private List<Vector3> points = new List<Vector3>();
    private Vector3 lastPoint;
    private bool isActive;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.useWorldSpace = true;
    }

    void Update()
    {
        if (!isActive) return;

        Vector3 groundPos;
        if (TryGetGroundPosition(out groundPos))
        {
            if (points.Count == 0 || Vector3.Distance(lastPoint, groundPos) >= pointSpacing)
            {
                AddPoint(groundPos);
            }
        }
    }

    bool TryGetGroundPosition(out Vector3 groundPos)
    {
        Ray ray = new Ray(player.position + Vector3.up, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f, groundLayer))
        {
            groundPos = hit.point + Vector3.up * groundOffset;
            return true;
        }

        groundPos = Vector3.zero;
        return false;
    }

    void AddPoint(Vector3 point)
    {
        points.Add(point);
        line.positionCount = points.Count;
        line.SetPosition(points.Count - 1, point);
        lastPoint = point;
    }

    public void StartTrail()
    {
        isActive = true;
    }

    public void StopTrail()
    {
        isActive = false;
    }
}
