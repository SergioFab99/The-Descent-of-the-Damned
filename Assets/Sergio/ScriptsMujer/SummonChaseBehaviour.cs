using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class SummonChaseBehaviour : MonoBehaviour
{
    [SerializeField] private float chaseDelay = 1f;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float stopDistance = 1.25f;
    [SerializeField] private float yawOffset = 0f;
    [SerializeField] private NavMeshAgent agent;

    private Transform target;
    private bool canChase;

    private Quaternion rotationOffset;

    private void Awake()
    {
        rotationOffset = transform.rotation;
    }

    public void Initialize(Transform targetToChase)
    {
        agent=GetComponent<NavMeshAgent>();
        
        target = targetToChase;
        //rotationOffset = transform.rotation;
        
    }

    private IEnumerator StartChaseAfterDelay()
    {
        canChase = false;
        yield return new WaitForSeconds(chaseDelay);
        canChase = true;
    }

    private void Update()
    {
        if (agent == null) return;
        agent.enabled = true;
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
            StartCoroutine(StartChaseAfterDelay());
            return;
        }
        
        return;
        if (!canChase || target == null) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float sqrStop = stopDistance * stopDistance;
        if (toTarget.sqrMagnitude <= sqrStop) return;

        if (toTarget.sqrMagnitude > 0.0001f)
        {
            float desiredYaw = Quaternion.LookRotation(toTarget.normalized, Vector3.up).eulerAngles.y + yawOffset;
            Quaternion desiredRotation = Quaternion.Euler(0f, desiredYaw, 0f) * rotationOffset;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }

        Vector3 direction = toTarget.normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
