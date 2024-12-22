using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private Animator anim;

    private float canSwing;
    public float canSwingTime = .5f;
    public float swingRadius = 1f;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire2") && canSwing < Time.time)
        {
            //swing sword
            canSwing = Time.time + canSwingTime;
            SwingSword();
        }
    }
    void SwingSword()
    {
        //play animation
        anim.SetTrigger("Swing");
        //detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position + offset, swingRadius);
        foreach(Collider2D enemy in hitEnemies)
        {
            if(enemy.CompareTag("Enemy"))
            {
                Debug.Log("Hit: " + enemy.name);
                //damage enemies
            }
        }
    }
}
