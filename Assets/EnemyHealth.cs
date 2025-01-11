using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public float knockbackDuration = 1f;
    public bool isGettingKnocked;
    public float knockbackGravity;
    private Vector2 knockbackVelocity;
    private float startingKnockbackGravity;
    public float bounciness = 2f;
    private float startingBounciness;

    private Animator anim;
    private Rigidbody2D rb;
    public LayerMask wallLayer;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startingKnockbackGravity = rb.gravityScale;
        startingBounciness = rb.sharedMaterial.bounciness;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGettingKnocked)
        {
            rb.gravityScale = knockbackGravity;
            rb.sharedMaterial.bounciness = bounciness;
        }
        else
        {
            rb.gravityScale = startingKnockbackGravity * 2;
            rb.sharedMaterial.bounciness = startingBounciness;
        }
    }
    //take damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Knockback(Vector3 direction, float force)
    {
        isGettingKnocked = true;
        anim.ResetTrigger("Attack");
        anim.SetBool("isKnocked", true);
        //add knockback to enemy
        rb.velocity = Vector2.zero;
        knockbackVelocity = direction * force;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        StartCoroutine(EndKnockback(knockbackDuration));
    }
    IEnumerator EndKnockback(float duration)
    {
        yield return new WaitForSeconds(duration);
        isGettingKnocked = false;
        anim.SetBool("isKnocked", false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGettingKnocked && ((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            // Reflect knockback direction upon hitting a wall
            Vector2 normal = collision.contacts[0].normal;
            knockbackVelocity = Vector2.Reflect(knockbackVelocity / 1.5f, normal);
            rb.velocity = knockbackVelocity;
        }
    }
    private void Die()
    {
        Debug.Log("Enemy died");
    }
}

