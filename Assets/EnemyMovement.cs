using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public float[] patrolXPoints;   // Array of patrol points
    public Transform player;           // Reference to the player
    public LayerMask whatIsGround;     // Layer mask to identify ground
    public float detectionRange = 10f; // Range to detect the player
    public float attackRange = 2f;     // Range to attack the player
    public int attackDamage = 10;      // Damage dealt to the player
    public float attackCooldown = 1f; // Time between attacks
    public bool isPlayerVisible;

    public Transform[] firePoints;
    public GameObject bulletPrefab;

    private Animator animator;         // Animator for enemy animations
    private float lastAttackTime;      // Time since the last attack

    void Start()
    {
        animator = GetComponent<Animator>();
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
        if (isPlayerVisible)
        {
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        // Check attack cooldown
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
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
    public void SpawnBullets()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            Vector3 bulletDirection = (player.position - firePoints[i].position).normalized;

            // Calculate 2D rotation
            float angle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);

            // Instantiate bullet with the correct rotation
            GameObject bulletInstance = Instantiate(bulletPrefab, firePoints[i].position, bulletRotation);

            Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(bulletDirection);
            }
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
    }
}