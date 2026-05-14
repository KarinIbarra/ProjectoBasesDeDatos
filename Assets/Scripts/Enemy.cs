using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    protected NavMeshAgent agent;

    [Header("Velocidades")]
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;

    [Header("Visión")]
    public float viewDistance = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public float closeDetectionRadius = 2f;
    public LayerMask obstacleMask;

    [Header("Combate")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    [Header("Memoria")]
    public float memoryDuration = 5f;
    private float memoryTimer;

    [Header("Patrulla")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex;

    private EnemyState currentState;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        ChangeState(new EnemyPatrolState());
    }   

    protected virtual void Update()
    {
        currentState?.UpdateState(this);
    }

    #region STATE MACHINE
    public void ChangeState(EnemyState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);  
    }
    #endregion

    #region DETECTION
    public bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 enemyEye = transform.position + Vector3.up * 1.5f;
        Vector3 playerTarget = player.position + Vector3.up * 1f;
        float distance = Vector3.Distance(enemyEye, playerTarget);

        if (distance <= closeDetectionRadius)
        {
            if (!Physics.Raycast(enemyEye, (playerTarget - enemyEye).normalized,
                distance, obstacleMask))
                return true;
        }

        Vector3 directionToPlayer = (playerTarget - enemyEye).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle <= viewAngle / 2f && distance <= viewDistance)
        {
            if (!Physics.Raycast(enemyEye, directionToPlayer, distance, obstacleMask))
                return true;
        }

        return false;
    }
    #endregion

    #region MEMORY
    public void StartMemory() { memoryTimer = memoryDuration; }
    public void ResetMemory() { memoryTimer = memoryDuration; }
    public void UpdateMemory() { memoryTimer -= Time.deltaTime; }
    public bool MemoryExpired() { return memoryTimer <= 0f; }
    #endregion

    #region MOVEMENT
    public void MoveTo(Vector3 position, float speed)
    {
        if (!agent.enabled) return;

        agent.isStopped = false;   
        agent.speed = speed;
        agent.SetDestination(position);
    }

    public void StopMoving()
    {
        if (!agent.enabled) return;

        agent.isStopped = true;  
    }

    public Transform GetNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return null;
        Transform point = patrolPoints[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        return point;
    }
    #endregion

    public void AlertToPosition(Vector3 position)
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(position);

        if (this is RangedEnemy)
            ChangeState(new RangedChaseState());
        else
            ChangeState(new EnemyChaseState());
    }
    public void EnableEscapeMode(float multiplier)
    {
        patrolSpeed *= multiplier;
        chaseSpeed *= multiplier;
        if (agent != null) agent.speed *= multiplier; 
    }
}