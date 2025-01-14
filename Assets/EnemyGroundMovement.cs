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
    public float attackWalkSpeed = 10f;
    public float waitTime = 1f;        //time waiting between patrol points
    private bool isWaiting = false;
    public bool isPlayerVisible;
    public bool isGrounded;
    private PlayerMovement pMove;
    public Transform groundCheck;
    public Transform cameraPoint;

    public Transform firePoint;
    public GameObject bulletPrefab;

    private NavMeshAgent agent;        // NavMeshAgent for enemy movement
    private Animator animator;         // Animator for enemy animations
    private int currentPatrolIndex;    // Current patrol waypoint index
    private float lastAttackTime;      // Time since the last attack
    private EnemyHealth stats;

    private enum EnemyState { Patrolling, Chasing, Attacking, Knocked }
    private EnemyState currentState;   // Current state of the enemy

    void Start()
    {
        pMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        stats = GetComponent<EnemyHealth>();
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
        isPlayerVisible = hit.collider != null && hit.collider.gameObject == player.gameObject;

        // Update the enemy's state based on visibility and distance
        if (isPlayerVisible && !stats.isGettingKnocked)
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
        else if (!isPlayerVisible && !stats.isGettingKnocked)
        {
            currentState = EnemyState.Patrolling;
        }
        else
        {
            currentState = EnemyState.Knocked;
        }

        // Perform actions based on the current state
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, .4f, whatIsGround);
        if (isGrounded)
        {
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

        if(cameraPoint == pMove.currentCameraPoint)
        {
            ClampEnemyWithinCameraBounds();
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

        if (!isWaiting)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(patrolXPoints[currentPatrolIndex], transform.position.y), patrolSpeed * Time.deltaTime);
            animator.SetBool("isWalking", true);
            //animator.SetBool("isRunning", false);
        }
    }

    void ChasePlayer()
    {
        // Set the player's position as the destination
        transform.position = Vector2.MoveTowards(transform.position, player.position, attackWalkSpeed * Time.deltaTime);
        //animator.SetBool("isWalking", false);
        //animator.SetBool("isRunning", true);
    }

    void AttackPlayer()
    {
        // Determine the horizontal direction to the player
        float direction = player.position.x - transform.position.x;

        // Flip the enemy to face the player
        /*if (direction > 0)
        {
            // Face right
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (direction < 0)
        {
            // Face left
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }*/

        // Check attack cooldown
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack"); // Trigger attack animation
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
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(waitTime);
        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolXPoints.Length)
        {
            currentPatrolIndex = 0;
        }

        isWaiting = false;
    }
    void ClampEnemyWithinCameraBounds()
    {
        //get camera bounds
        Camera mainCamera = Camera.main;
        Vector3 minScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 maxScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        //clamp the enemy position within bounds
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minScreenBounds.x, maxScreenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minScreenBounds.y, maxScreenBounds.y);

        transform.position = clampedPosition;
    }
    public void SpawnBullet()
    {
        Vector3 bulletDirection = (player.position - firePoint.position).normalized;

        // Calculate 2D rotation
        float angle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);

        // Instantiate bullet with the correct rotation
        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(bulletDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < patrolXPoints.Length; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(new Vector3(patrolXPoints[i], transform.position.y, transform.position.z), .15f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(groundCheck.position, .4f);
    }
}