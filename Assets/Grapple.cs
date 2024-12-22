using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    private Rigidbody2D rb;
    public LayerMask whatIsGrapple;
    public LineRenderer lineRenderer;
    private PlayerMovement playerMovement;

    public AudioSource clip;
    public float[] pitches;

    public Transform firePoint;
    private Vector3 dir;

    private Vector3 grapplePoint;
    public float grappleRange = 5f;
    public float grappleForce = 10f;

    private bool isGrappling;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            GrappleShoot();
        }
        else if(Input.GetButtonUp("Fire1"))
        {
            lineRenderer.enabled = false;
            isGrappling = false;
        }
    }
    void GrappleShoot()
    {
        //get mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        //get direction vector
        dir = (mousePos - firePoint.position).normalized;

        //shoot ray in that direction
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, dir, grappleRange, whatIsGrapple);
        if(hitInfo)
        {
            //hit something
            grapplePoint = hitInfo.point;

            //play sound
            int index = Random.Range(0, pitches.Length);
            clip.pitch = pitches[index];
            clip.Play();

            isGrappling = true;
        }
        else
        {
            //dont hit something, still want to fire a line
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + dir * 100f);
        }

        lineRenderer.enabled = true;
    }
    void FixedUpdate()
    {
        if (isGrappling)
        {
            //pull player towards grapple point
            //the direction will change so we can't just use the initial direction from before
            Vector3 currentDir = (grapplePoint - firePoint.position).normalized;
            rb.AddForce(currentDir * grappleForce, ForceMode2D.Force);

            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, grapplePoint);

            playerMovement.anim.SetBool("isJumping", true);
            playerMovement.isJumping = true;
        }
    }
}
