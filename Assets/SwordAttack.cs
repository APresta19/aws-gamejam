using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private Animator anim;
    public Transform player;
    private TrailRenderer slashEffect;
    private float mouseAngle;

    private float canSwing;
    public float canSwingTime = 0.5f;
    public float swingRadius = 1f;
    public Vector3 hitPointOffset;

    private float angleOffset; //adjusts the direction of the sword based on mouse pos
    public float angleOffsetAmount = 90f; //starting sword direction
    private float facingLeftOffset; //facing left needs some adjustment
    public float facingLeftOffsetNumber = 30f;
    public float swingRange = 90f; //angle at which the sword swings (larger num --> longer swing)
    private bool hasSwung;

    void Start()
    {
        anim = GetComponent<Animator>();
        slashEffect = GameObject.FindObjectOfType<TrailRenderer>();
        angleOffset = angleOffsetAmount;
        facingLeftOffset = facingLeftOffsetNumber;
        hasSwung = true;
    }

    void Update()
    {
        // Get mouse position and direction vector
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = (player.position - mousePos).normalized;
        //make sure sword follows the right direction when facing right/left
        mouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float swingDir = mouseAngle + angleOffset + facingLeftOffset;
        if(player.GetComponent<PlayerMovement>().facingLeft)
        {
            mouseAngle = -mouseAngle;
            facingLeftOffset = facingLeftOffsetNumber;
            transform.localRotation = Quaternion.Euler(0, 0, -swingDir);
        }
        else
        {
            facingLeftOffset = 0;
            transform.localRotation = Quaternion.Euler(0, 0, swingDir);
        }
        //need to change this line -- only set if not swinging
        
        //Debug.Log(swingDir);
        if (Input.GetButtonDown("Fire2") && canSwing < Time.time)
        {
            canSwing = Time.time + canSwingTime;
            angleOffset -= swingRange;
            swingDir = mouseAngle + angleOffset + facingLeftOffset;

            //need to negate mouseAngle for facing left again
            //FlipSword();

            transform.localRotation = Quaternion.Euler(0, 0, swingDir);
            SwingSword();
        }

        // Reset angleOffset to default when the animation ends
        if (!hasSwung && anim.GetCurrentAnimatorStateInfo(0).IsName("Sword_Slash") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .99f)
        {
            angleOffset = angleOffsetAmount;
            Debug.Log("Animation ended");
            hasSwung = true;
            //FlipSword();
        }
    }

    void SwingSword()
    {
        hasSwung = false;

        //trigger animation
        anim.SetTrigger("Swing");

        // Detect enemies in range at the start of the swing
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position + hitPointOffset, swingRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log("Hit: " + enemy.name);
                // Damage the enemy
            }
        }
        
        StartCoroutine(ResetTrigger("Swing"));
    }

    public void FlipSword()
    {
        if (player.GetComponent<PlayerMovement>().facingLeft)
        {
            transform.localScale *= -1;
        }
    }
    IEnumerator ResetTrigger(string triggerName)
    {
        yield return null; // Wait one frame to ensure the animation starts
        anim.ResetTrigger(triggerName);
    }
    void ShowSlashEffect()
    {
        slashEffect.enabled = true;
    }

    void HideSlashEffect()
    {
        slashEffect.enabled = false;
    }


    private void OnDrawGizmos()
    {
        // Visualize the sword's hit range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + hitPointOffset, swingRadius);
    }
}
