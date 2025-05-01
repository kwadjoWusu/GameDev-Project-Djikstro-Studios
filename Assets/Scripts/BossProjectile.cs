using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 5f;
    public GameObject hitEffectPrefab;
    
    private void Start()
    {
        // Destroy after lifetime expires
        Destroy(gameObject, lifeTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if projectile hit player
        if (collision.CompareTag("Player"))
        {
            // Try to damage player
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // The TakeDamage method will be triggered by PlayerHealth's OnTriggerEnter2D
                // No need to call it directly here
            }
            
            // Create hit effect
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
        // Destroy projectile if it hits terrain/platforms
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Create hit effect
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
    }
}