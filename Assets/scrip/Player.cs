using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public int maxJumps = 2; 
    private int jumpCount = 0;
    public float climbSpeed = 4f;

    public float dashSpeed = 15f;
    public float dashTime = 0.2f;
    private bool isDashing = false;
    private float dashTimer;
    public float dashCooldown = 10f;
    private float lastDashTime = -999f;

    private Animator animator;
    private Rigidbody2D rb;

    private bool isClimbing = false;
    private bool isOnLadder = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.L) && moveX != 0 && !isDashing)
        {
            isDashing = true;
            dashTimer = dashTime;
            lastDashTime = Time.time;
            rb.linearVelocity = new Vector2(moveX > 0 ? dashSpeed : -dashSpeed, 0f);
            animator.SetTrigger("isDash");
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
            return;
        }

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(moveX * moveSpeed, moveY * climbSpeed);
            rb.gravityScale = 0f;
            animator.SetBool("isClimbing", Mathf.Abs(moveY) > 0.1f || Mathf.Abs(moveX) > 0.1f);
        }
        else
        {
            Vector2 movement = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = movement;   
            rb.gravityScale = 1f;
        }

        if (Mathf.Abs(moveX) > 0.1f)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        animator.SetFloat("Speed", Mathf.Abs(moveX));

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            animator.SetBool("isJumping", true);
        }

        if (rb.linearVelocity.y < 0)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            jumpCount = 0;  
        }

        if (moveX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);  
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);  
        }

        if (isOnLadder && Mathf.Abs(moveY) > 0.1f)
        {
            isClimbing = true;
            jumpCount = 0; 
        }

        if (!isOnLadder)
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
            isClimbing = false;
            animator.SetBool("isClimbing", false);
        }
    }


}

