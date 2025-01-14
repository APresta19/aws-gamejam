using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private TheWall theWall;
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
    public GameObject bulletEffect;
    // Start is called before the first frame update
    void Start()
    {
        theWall = GameObject.Find("The Wall").GetComponent<TheWall>();
        currentHealth = maxHealth;
        if(transform.CompareTag("Enemy"))
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            startingKnockbackGravity = rb.gravityScale;
            startingBounciness = rb.sharedMaterial.bounciness;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.CompareTag("Enemy"))
        {
            if (isGettingKnocked)
            {
                anim.SetBool("isKnocked", true);
                rb.gravityScale = knockbackGravity;
                rb.sharedMaterial.bounciness = bounciness;
            }
            else
            {
                anim.SetBool("isKnocked", false);
                rb.gravityScale = startingKnockbackGravity;
                rb.sharedMaterial.bounciness = startingBounciness;
            }
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Bullet") && gameObject.CompareTag("FloatingEnemy") && other.GetComponent<SpriteRenderer>().material.name.Contains(other.GetComponent<Bullet>().reverseMat.name))
        {
            Debug.Log("Hit FLOATING ENEMY");
            GameObject ins = Instantiate(bulletEffect, other.transform.position, Quaternion.identity);
            TakeDamage(other.GetComponent<Bullet>().damage);
            theWall.CheckForWin(transform.gameObject);

            Destroy(ins, 1f);
            Destroy(other.gameObject);
        }
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}
