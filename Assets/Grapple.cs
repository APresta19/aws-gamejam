using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    private Camera cam;
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
    private bool isDoingLine;
    public float duration = 0.2f;

    private void Start()
    {
        cam = Camera.main;
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
        ClampCameraToBounds();
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
        if (hitInfo)
        {
            //hit something
            grapplePoint = hitInfo.point;

            //play sound
            int index = Random.Range(0, pitches.Length);
            clip.pitch = pitches[index];
            clip.Play();

            isGrappling = true;
            DoLine(true);
        }
        else
        {
            DoLine();
        }
    }
    public void DoLine(bool isGrappling = false)
    {
        if (!isDoingLine)
        {
            lineRenderer.enabled = true;
            StartCoroutine(LineRoutine(isGrappling));
        }
    }

    IEnumerator LineRoutine(bool isGrappling)
    {
        isDoingLine = true;

        lineRenderer.SetPosition(0, firePoint.position);

        float timePassed = 0f;
        while (timePassed < duration)
        {
            float factor = timePassed / duration;
            // optionally add ease-in ease-out
            //factor = Mathf.SmoothStep(0, 1, factor);
            if(!isGrappling)
                lineRenderer.SetPosition(1, Vector3.Lerp(firePoint.position, firePoint.position + dir * grappleRange, factor));
            else
                lineRenderer.SetPosition(1, Vector3.Lerp(firePoint.position, grapplePoint, factor));
            //timePassed += Mathf.Min(Time.deltaTime, duration - timePassed);
            timePassed += Time.deltaTime;
            yield return null;
        }

        if (isGrappling)
        {
            lineRenderer.SetPosition(1, grapplePoint);
        }
        else
        {
            lineRenderer.SetPosition(1, firePoint.position + dir * grappleRange);
            lineRenderer.enabled = false; // Disable if not grappling
        }

        isDoingLine = false;
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
    void ClampCameraToBounds()
    {
        //calculate camera bounds
        float cameraHalfWidth = cam.orthographicSize * cam.aspect;
        float cameraHalfHeight = cam.orthographicSize;

        //set bounds (1 for offset)
        float leftBound = cam.transform.position.x - cameraHalfWidth + 1;
        float rightBound = cam.transform.position.x + cameraHalfWidth - 1;
        float bottomBound = cam.transform.position.y - cameraHalfHeight + 1;
        float topBound = cam.transform.position.y + cameraHalfHeight - 1;

        //clamp the player's position within bounds
        Vector3 clampedPosition = rb.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, leftBound, rightBound);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, bottomBound, topBound);

        //update Rigidbody position
        rb.position = clampedPosition;
    }
}
