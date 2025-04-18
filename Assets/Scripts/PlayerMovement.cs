using System.Collections;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public ParticleSystem smokeFX;
    public Rigidbody2D rb;
    public Animator animator;
    bool isFacingRight = true;
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float horizontalMovement;
    BoxCollider2D playerCollider;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    private int jumpsRemaining;
    private bool hasUsedWallJump = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public Vector2 groundCheckRadius = new Vector2(0.49f, 0.03f);
    public LayerMask groundLayer;
    bool isGrounded;
     bool isOnPlatform;

    [Header("Wall Check")]
    public Transform wallCheck;
    public Vector2 wallCheckRadius = new Vector2(0.49f, 0.03f);
    public LayerMask wallLayer;
    bool isWallTouching;

    [Header("Wall Movement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;

    [Header("Wall Jump")]
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.4f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(8f, 16f);
    private float wallJumpCooldown = 0f;

    [Header("Gravity")]
    public float baseGravity = 7f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2.5f;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        jumpsRemaining = maxJumps;
        playerCollider= GetComponent<BoxCollider2D>();

    }


    void Update()
    {
        // Update animations
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);

        if(isDashing)
        {
            return;
        }
        GroundCheck();
        Gravity();
        CheckWallTouch();
        WallSlide();

        // Handle movement and wall jumping
        if (isWallJumping)
        {
            // During wall jump, let the WallJump method control movement
            WallJump();
        }
        else
        {
            // Normal movement
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            Flip();
        }

        // Decrease wall jump cooldown
        if (wallJumpCooldown > 0)
        {
            wallJumpCooldown -= Time.deltaTime;
        }

    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void CheckWallTouch()
    {
        isWallTouching = Physics2D.OverlapBox(wallCheck.position, wallCheckRadius, 0, wallLayer);
    }

    private void WallSlide()
    {
        bool wasWallSliding = isWallSliding;

        if (!isGrounded && isWallTouching && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));

            // Reset jumps when wall sliding starts
            if (!wasWallSliding)
            {
                jumpsRemaining = maxJumps;
            }
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        // Apply wall jump movement during the wall jump timer duration
        rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, rb.linearVelocity.y);
        wallJumpTimer -= Time.deltaTime;

        if (wallJumpTimer <= 0)
        {
            isWallJumping = false;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
        wallJumpTimer = 0;
        hasUsedWallJump = true;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }


    public void Dash(InputAction.CallbackContext context)
    {
       if(context.performed && canDash)
       {
        StartCoroutine(DashCoroutine());
       }
    }

   
    private IEnumerator DashCoroutine()
    {

        Physics2D.IgnoreLayerCollision(8,9,true);

        canDash = false;
        isDashing=true;
        trailRenderer.emitting = true;

        float dashDirection = isFacingRight ? 1f:-1f;

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed,rb.linearVelocity.y);//Dash movement

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);//Reset horizontal velocity

        isDashing=false;
        trailRenderer.emitting=false;
        Physics2D.IgnoreLayerCollision(8,9,false);


        yield return new WaitForSeconds(dashCooldown);
        canDash= true;
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && isOnPlatform && playerCollider.enabled)
        {
            //Coroutine Dropping
            StartCoroutine(DisablePlayerCollider(0.25f));

        }
    }

    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled=false;
        yield return new WaitForSeconds(disableTime);
        playerCollider.enabled=true;


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform=true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
         if(collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform=false;
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        // Wall Jump logic - completely separate from regular jumps
        if (context.performed && isWallSliding && wallJumpCooldown <= 0)
        {
            // Execute wall jump
            isWallJumping = true;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;
            wallJumpCooldown = 0.5f; // Add cooldown to prevent immediate re-wall jumping

            // Apply immediate wall jump velocity
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            JumpFX();

            // Flip character if needed
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime);
            return; // Exit early to avoid regular jump logic
        }

        // Regular Jump logic - only executes if we're not wall jumping
        if (context.performed && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            jumpsRemaining--;
            JumpFX();
        }
        else if (context.canceled && rb.linearVelocity.y > 0)
        {
            // Variable jump height when button is released early
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void JumpFX()
    {
        animator.SetTrigger("jump");
        smokeFX.Play();
    }

    private void GroundCheck()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckRadius, 0, groundLayer);

        // Reset jumps and wall jump status when touching the ground
        if (isGrounded && !wasGrounded)
        {
            jumpsRemaining = maxJumps;
            hasUsedWallJump = false;
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
            if (rb.linearVelocity.y == 0)
            {
                smokeFX.Play();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheck.position, wallCheckRadius);
    }
}