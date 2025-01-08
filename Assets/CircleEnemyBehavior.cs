using UnityEngine;

public class CircleEnemyBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f; // Speed at which the enemy moves
    public Transform[] patrolPoints; // Array of patrol points for patrolling
    private int currentPointIndex = 0; // Index of the current patrol point

    [Header("Player Detection")]
    public float detectionRadius = 5f; // Radius to detect the player
    private Transform player; // Reference to the player's Transform
    private bool playerDetected = false;

    [Header("Health Settings")]
    public int health = 3; // Enemy's health

    private void Start()
    {
        // Find the player in the scene by tag
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points set for CircleEnemy.");
        }
    }

    private void Update()
    {
        // Check for player detection
        if (Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            playerDetected = true;
        }
        else
        {
            playerDetected = false;
        }

        // Decide behavior based on player detection
        if (playerDetected)
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // Move towards the current patrol point
        Transform targetPoint = patrolPoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // If reached the patrol point, switch to the next one
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    private void FollowPlayer()
    {
        // Move towards the player's position
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject); // Destroy the enemy when health is 0
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the detection radius in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
