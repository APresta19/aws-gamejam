using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : MonoBehaviour
{
    public Transform[] patrolPoints;   // Array of patrol points
    public LayerMask whatIsGround;     // Layer mask to identify ground
    public float detectionRange = 10f; // Range to detect the player
    public float shootInterval = 2f;   // Time between shots
    public GameObject projectile;      // Projectile prefab
    public Transform firePoint;        // Point where bullets are spawned
    public float patrolSpeed = 5f;     // Patrol speed
    public float waitTime = 1f;        // Time waiting between patrol points

    private int currentPatrolIndex;    // Current patrol waypoint index
    private bool isWaiting = false;
    private float shootTimer;
    private Animator animator;
    private EnemyHealth stats;

    private enum EnemyState { Patrolling, Shooting }
    private EnemyState currentState;   // Current state of the enemy

    void Start()
    {
        animator = GetComponent<Animator>();
        stats = GetComponent<EnemyHealth>();
        currentPatrolIndex = 0;
        currentState = EnemyState.Patrolling;
        shootTimer = shootInterval;
    }

    void Update()
    {
        // Perform actions based on the current state
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Shooting:
                Shoot();
                break;
        }
    }

    void Patrol()
    {
        // Move to the next patrol point
        if (!isWaiting)
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolPoints[currentPatrolIndex].position, patrolSpeed * Time.deltaTime);
            animator.SetBool("isWalking", true);

            // If the enemy has reached the current waypoint
            if (Vector2.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.2f)
            {
                StartCoroutine(WaitUntilNextWaypoint());
                isWaiting = true;
            }
        }
    }

    IEnumerator WaitUntilNextWaypoint()
    {
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(waitTime);
        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolPoints.Length)
        {
            currentPatrolIndex = 0;
        }
        isWaiting = false;
    }

    void Shoot()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            animator.SetTrigger("Shoot");
            Instantiate(projectile, firePoint.position, firePoint.rotation);
            shootTimer = shootInterval;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        foreach (Transform point in patrolPoints)
        {
            Gizmos.DrawSphere(point.position, 0.2f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}