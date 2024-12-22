using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;

    public LayerMask whatIsGround, whatIsWall;
    [HideInInspector] public bool isJumping;
    private bool isLanding;
    private bool facingRight;
    private bool canWallJump;

    public Transform groundCheck, wallCheck;
    public float wallCheckRadius = 1f;

    private Vector2 move;

    private float lastPressedJump;
    private float lastGrounded;

    public float moveSpeed = 10f;
    public float maxSpeed = 10f;
    public float jumpForce = 5f;
    //public float vecGravity = .5f;
    public float jumpTime = 2f;
    public float jumpLedgeTime = .5f;
    private float jumpCounter;
    public float acceleration = 2f, decceleration = 1f;
    public float velPower = 5f;
    public float wallVelocity = 2f;

    private float flipTimer;
    public float flipCooldown = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        move.x = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(move.x));

        if (isGrounded())
        {
            lastGrounded = jumpLedgeTime;

            if (Mathf.Abs(rb.velocity.y) < .1f)
            {
                anim.SetBool("isJumping", false);
                isJumping = false;

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Fall"))
                {
                    isLanding = false;
                    anim.SetBool("isLanding", false);
                }
                else
                {
                    isLanding = true;
                    anim.SetBool("isLanding", true);
                }
            }
        }
        else
        {
            lastGrounded -= Time.deltaTime;
        }

        canWallJump = isWalled() && !isGrounded();
        if (Input.GetButtonDown("Jump") && (isGrounded() || lastGrounded > 0 || canWallJump))
        {
            Jump();
        }

        //jumping and rising
        if (isJumping && rb.velocity.y > 0)
        {
            //start jump counter
            jumpCounter += Time.deltaTime;
            if(jumpCounter < jumpTime)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        if(Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        //flip player
        flipTimer -= Time.deltaTime;
        if (flipTimer <= 0)
        {
            if (move.x > 0 && facingRight)
            {
                FlipPlayer();
            }
            else if (move.x < 0 && !facingRight)
            {
                FlipPlayer();
            }
        }
    }
    private void FixedUpdate()
    {
        float targetSpeed = move.x * moveSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accRate, velPower) * Mathf.Sign(speedDiff);

        //wall slide --> falling
        if(move.x != 0 && isWalled() && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallVelocity));
        }

        //clamp movement
        if (Mathf.Abs(rb.velocity.x) < maxSpeed || Mathf.Sign(rb.velocity.x) != Mathf.Sign(targetSpeed))
        {
            rb.AddForce(movement * Vector2.right);
        }
    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isJumping = true;
        jumpCounter = 0;
        lastGrounded = 0;
        anim.SetBool("isJumping", true);
    }
    bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .3f, whatIsGround);
    }
    bool isWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsWall);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
    }
    void FlipPlayer()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        flipTimer = flipCooldown;
    }
}
