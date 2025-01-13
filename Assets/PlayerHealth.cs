using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Image fill;
    public Slider healthSlider;
    public Gradient healthGrad;
    public Animator deathAnimator;
    private bool isDead;
    public AudioSource hurtSound;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //take damage
    public void TakeDamage(int damage)
    {
        hurtSound.Play();
        GetComponent<Animator>().SetTrigger("Hurt");

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthSlider.value = currentHealth;
        Color color = healthGrad.Evaluate((float)currentHealth / maxHealth);
        fill.color = color;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    private void Die()
    {
        isDead = true;
        deathAnimator.SetTrigger("FadeIn");
    }
}
