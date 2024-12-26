using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] patrolPoints;    // Array of patrol waypoints
    public Transform player;           // Reference to the player
    public LayerMask whatIsGround;     // Layer mask to identify ground
    public float detectionRange = 10f; // Range to detect the player
    public float attackRange = 2f;     // Range to attack the player
    public int attackDamage = 10;      // Damage dealt to the player
    public float attackCooldown = 1f; // Time between attacks

    private NavMeshAgent agent;        // NavMeshAgent for enemy movement
    private Animator animator;         // Animator for enemy animations
    private int currentPatrolIndex;    // Current patrol waypoint index
    private float lastAttackTime;      // Time since the last attack

    private enum EnemyState { Patrolling, Chasing, Attacking }
    private EnemyState currentState;   // Current state of the enemy

    void Start()
    {
        // Initialize the NavMeshAgent, Animator, and patrol settings
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
        currentPatrolIndex = 0;
        currentState = EnemyState.Patrolling;

        // Set the initial patrol destination
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Perform a raycast to check if the player is visible
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, whatIsGround);
        bool isPlayerVisible = hit.collider == null || hit.collider.gameObject == player.gameObject;

        // Update the enemy's state based on visibility and distance
        if (isPlayerVisible)
        {
            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attacking;
            }
            else if (distanceToPlayer <= detectionRange)
            {
                currentState = EnemyState.Chasing;
            }
        }
        else
        {
            currentState = EnemyState.Patrolling;
        }

        // Perform actions based on the current state
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.Attacking:
                AttackPlayer();
                break;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return; // No patrol points available

        // If the agent has reached the current waypoint
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Move to the next waypoint in the patrol route
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        // Set a slower movement speed during patrol
        agent.speed = 3f;
        //animator.SetBool("isWalking", true);
        //animator.SetBool("isRunning", false);
    }

    void ChasePlayer()
    {
        // Set the player's position as the destination and increase speed
        agent.SetDestination(player.position);
        agent.speed = 6f; // Increase speed for chasing
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
    }

    void AttackPlayer()
    {
        // Stop the agent's movement while attacking
        agent.ResetPath();

        // Face the player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        // Check attack cooldown
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            //animator.SetTrigger("Attack"); // Trigger attack animation
            lastAttackTime = Time.time;
        }
    }

    // Called via animation event during the attack animation
    public void DealDamageToPlayer()
    {
        // Check if player is in range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            // Assume the player has a health script
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage); // Apply damage
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection and attack ranges in the Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}