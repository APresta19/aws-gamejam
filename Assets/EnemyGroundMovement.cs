using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGroundMovement : MonoBehaviour
{
    public float[] patrolXPoints;   // Array of patrol points
    public Transform player;           // Reference to the player
    public LayerMask whatIsGround;     // Layer mask to identify ground
    public float detectionRange = 10f; // Range to detect the player
    public float attackRange = 2f;     // Range to attack the player
    public int attackDamage = 10;      // Damage dealt to the player
    public float attackCooldown = 1f; // Time between attacks
    public float patrolSpeed = 5f;     //patrol of enemy
    public float attackSpeed = 10f;
    public float waitTime = 1f;        //time waiting between patrol points
    private bool isWaiting = false;

    private NavMeshAgent agent;        // NavMeshAgent for enemy movement
    private Animator animator;         // Animator for enemy animations
    private int currentPatrolIndex;    // Current patrol waypoint index
    private float lastAttackTime;      // Time since the last attack

    private enum EnemyState { Patrolling, Chasing, Attacking }
    private EnemyState currentState;   // Current state of the enemy

    void Start()
    {
        currentPatrolIndex = 0;
        currentState = EnemyState.Patrolling;
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
        // If the agent has reached the current waypoint
        if (Vector2.Distance(transform.position, new Vector2(patrolXPoints[currentPatrolIndex], transform.position.y)) < .1f && !isWaiting)
        {
            // Stop for some time, then move to the next waypoint
            StartCoroutine(WaitUntilNextWaypoint());
            isWaiting = true;  
        }

        transform.position = Vector2.MoveTowards(transform.position, new Vector2(patrolXPoints[currentPatrolIndex], transform.position.y), patrolSpeed * Time.deltaTime);
        //animator.SetBool("isWalking", true);
        //animator.SetBool("isRunning", false);
    }

    void ChasePlayer()
    {
        // Set the player's position as the destination
        transform.position = Vector2.MoveTowards(transform.position, player.position, attackSpeed * Time.deltaTime);
        //animator.SetBool("isWalking", false);
       // animator.SetBool("isRunning", true);
    }

    void AttackPlayer()
    {
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
    IEnumerator WaitUntilNextWaypoint()
    {
        yield return new WaitForSeconds(waitTime);
        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolXPoints.Length)
        {
            currentPatrolIndex = 0;
        }

        isWaiting = false;
    }

    private void OnDrawGizmosSelected()
    {
        for(int i = 0; i < patrolXPoints.Length; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(new Vector3(patrolXPoints[i], transform.position.y, transform.position.z), .15f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
