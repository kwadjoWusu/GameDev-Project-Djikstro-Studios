using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private float horizontalMovement;
    private bool isRunning;
    private float runSpeed = 8.0f;
    private float walkSpeed = 5.0f;
    private bool facingRight = true;
    private bool isGrounded;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private float groundCheckRadius = 1.5f; // Ensure radius is reasonable

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGroundStatus();

        // Set animations
        animator.SetBool("isWalking", horizontalMovement != 0 && isGrounded);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", !isGrounded);

        // Flip player direction
        if (horizontalMovement > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalMovement < 0 && facingRight)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        // Move player
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRunning = true;
            moveSpeed = runSpeed;
        }
        else if (context.canceled)
        {
            isRunning = false;
            moveSpeed = walkSpeed;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void CheckGroundStatus()
    {
        Collider2D groundHit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = groundHit != null;
    }

    private void Flip()
    {
        // Flip player horizontally
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}