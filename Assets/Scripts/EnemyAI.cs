using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack }

    [Header("Estado actual (solo lectura)")]
    public State currentState = State.Patrol;

    [Header("Jugador")]
    public Transform player;              
    public string playerTag = "Player";

    [Header("Patrulla")]
    public Transform[] waypoints;         
    public float patrolSpeed = 2f;
    public float waitTime = 1.5f;
    public float randomPatrolRadius = 8f;

    [Header("Persecucion")]
    public float chaseSpeed = 4.5f;
    public float detectionRange = 10f;
    [Range(0, 360)] public float viewAngle = 140f;
    public float loseSightRange = 15f;

    [Header("Ataque")]
    public float attackRange = 1.8f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.2f;

    [Header("Visual (opcional)")]
    public Renderer bodyRenderer;         

    NavMeshAgent agent;
    Vector3 startPos;
    int waypointIndex;
    float waitTimer;
    float attackTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPos = transform.position;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else Debug.LogWarning("EnemyAI: no encontre ningun objeto con tag '" + playerTag + "'");
        }

        SetState(State.Patrol);
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                DoPatrol();
                if (CanSeePlayer(dist)) SetState(State.Chase);
                break;

            case State.Chase:
                agent.SetDestination(player.position);
                if (dist <= attackRange) SetState(State.Attack);
                else if (dist > loseSightRange) SetState(State.Patrol);
                break;

            case State.Attack:
                DoAttack(dist);
                if (dist > attackRange * 1.2f) SetState(State.Chase);
                break;
        }
    }

    void DoPatrol()
    {
        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waitTimer = 0f;
                GoToNextPatrolPoint();
            }
        }
    }

    void GoToNextPatrolPoint()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[waypointIndex].position);
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
        }
        else
        {
            Vector3 random = startPos + Random.insideUnitSphere * randomPatrolRadius;
            if (NavMesh.SamplePosition(random, out NavMeshHit hit, randomPatrolRadius, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }
    }

    bool CanSeePlayer(float dist)
    {
        if (dist > detectionRange) return false;

        Vector3 dir = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        return angle <= viewAngle * 0.5f;
    }

    void DoAttack(float dist)
    {
        // Mirar al jugador
        Vector3 look = player.position - transform.position;
        look.y = 0;
        if (look != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 10f * Time.deltaTime);

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown && dist <= attackRange * 1.2f)
        {
            attackTimer = 0f;
            // Llama a TakeDamage(float) en el jugador (si existe)
            player.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            Debug.Log("El enemigo ataca! Dano: " + attackDamage);
        }
    }

    void SetState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Patrol:
                agent.isStopped = false;
                agent.speed = patrolSpeed;
                agent.stoppingDistance = 0.3f;
                waitTimer = 0f;
                GoToNextPatrolPoint();
                SetColor(new Color(0.2f, 0.8f, 0.2f));   // verde
                break;

            case State.Chase:
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                agent.stoppingDistance = attackRange * 0.8f;
                SetColor(new Color(1f, 0.85f, 0.1f));    
                break;

            case State.Attack:
                agent.isStopped = true;
                attackTimer = attackCooldown;             
                SetColor(new Color(0.9f, 0.1f, 0.1f));   
                break;
        }
    }

    void SetColor(Color c)
    {
        if (bodyRenderer != null) bodyRenderer.material.color = c;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseSightRange);
    }
}
