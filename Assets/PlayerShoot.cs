using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Convert screen position to world position (with z=0 for 2D)
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Use Vector2 for 2D direction calculation
        Vector2 shootDirection = (mouseWorldPos - (Vector2)transform.position).normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Apply velocity
        bullet.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * bulletSpeed;

        // Destroy after delay
        Destroy(bullet, 2f);
    }
}
