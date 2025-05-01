using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyProjectilePrefab;
    public Transform projectileSpawnPoint;
    public ParticleSystem attackFX;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Movement")]
    public float followSpeed = 2f;
    public float detectRadius = 7f;
    public float jumpForce = 8f;
    public float jumpInterval = 2f;

    [Header("Attack")]
    public float attackCooldown = 1.5f;
    public float projectileSpeed = 8f;
    public int projectileDamage = 1;
    public float shootRadius = 10f;

    [Header("Health")]
    public int maxHealth = 10;
    public int currentHealth;
    public GameObject healthBarPrefab;
    private BossHealthBar healthBar;

    // References
    private Rigidbody2D rb;
    private Transform player;
    private bool facingRight = true;

    // Timers
    private float lastJumpTime;
    private float lastShootTime;
    private bool isGrounded;

    // Static instance
    public static BossController Instance { get; private set; }

    private void Awake()
    {
        // Set up singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // Don't continue with initialization if this is a duplicate
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;

        // Create health bar above boss
        GameObject healthBarObj = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
        healthBar = healthBarObj.GetComponent<BossHealthBar>();
        healthBar.SetMaxHealth(maxHealth);
        healthBar.transform.SetParent(transform);

        // Start jumping
        lastJumpTime = Time.time;

        // Register for reset event
        GameController.OnReset += ResetBoss;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events when destroyed
        GameController.OnReset -= ResetBoss;
    }

    private void Update()
    {
        if (player == null)
        {
            // Try to find player again if reference is lost
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null) return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Movement logic
        if (distanceToPlayer < detectRadius)
        {
            // Move towards player when nearby
            MoveTowardsPlayer();
        }

        // Jump periodically
        if (Time.time > lastJumpTime + jumpInterval && isGrounded)
        {
            Jump();
        }

        // Attack when player is in range
        if (distanceToPlayer < shootRadius && Time.time > lastShootTime + attackCooldown)
        {
            ShootProjectile();
        }

        // Update animations
    }

    private void MoveTowardsPlayer()
    {
        float horizontalDirection = player.position.x > transform.position.x ? 1 : -1;

        // Only move horizontally when grounded
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(horizontalDirection * followSpeed, rb.linearVelocity.y);
        }

        // Flip sprite based on movement direction
        if ((horizontalDirection > 0 && !facingRight) || (horizontalDirection < 0 && facingRight))
        {
            Flip();
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        lastJumpTime = Time.time;
        isGrounded = false;
    }

    private void ShootProjectile()
    {
        if (projectileSpawnPoint == null)
        {
            projectileSpawnPoint = transform;
        }

        Vector2 directionToPlayer = (player.position - projectileSpawnPoint.position).normalized;
        GameObject projectile = Instantiate(enemyProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        // Setup projectile
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.linearVelocity = directionToPlayer * projectileSpeed;

        // Setup damage component
        BossProjectile projectileScript = projectile.GetComponent<BossProjectile>();
        if (projectileScript == null)
        {
            projectileScript = projectile.AddComponent<BossProjectile>();
        }
        projectileScript.damage = projectileDamage;

        // Play attack effect
        if (attackFX != null)
        {
            attackFX.Play();
        }

        lastShootTime = Time.time;

        // Destroy projectile after some time
        Destroy(projectile, 5f);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if boss landed on ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Update health bar
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth);
        }

        // Visual feedback
        StartCoroutine(FlashRedCoroutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRedCoroutine()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        // You could add particle effects, sound, score increase, etc. here

        // Unsubscribe from events
        GameController.OnReset -= ResetBoss;

        // Disable components but keep object for death animation
        GetComponent<Collider2D>().enabled = false;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;

        // Clear the static instance if this is the current instance
        if (Instance == this)
        {
            Instance = null;
        }

        // Destroy after animation plays (or use animation event)
        Destroy(gameObject, 2f);
    }

    private void ResetBoss()
    {
        // Safe check to prevent errors
        if (this == null || gameObject == null) return;

        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        // Visualize attack radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRadius);
    }
}