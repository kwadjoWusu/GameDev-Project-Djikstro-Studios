using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 2f;
    public GameObject hitEffectPrefab;
    
    private void Start()
    {
        // Destroy after lifetime expires
        Destroy(gameObject, lifeTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if bullet hit boss
        BossController boss = collision.GetComponent<BossController>();
        if (boss != null)
        {
            // Damage the boss
            boss.TakeDamage(damage);
            
            // Create hit effect
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            
            // Destroy bullet
            Destroy(gameObject);
        }
        // Check if bullet hit enemy
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            // If you have an enemy damage/health system, call it here
            
            // Create hit effect
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            
            // Destroy bullet
            Destroy(gameObject);
        }
        // Destroy bullet if it hits terrain/platforms
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Create hit effect
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            
            // Destroy bullet
            Destroy(gameObject);
        }
    }
}