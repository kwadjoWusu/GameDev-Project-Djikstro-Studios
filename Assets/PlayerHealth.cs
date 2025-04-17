using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth =3;
    private int currentHealth;
    public HealthUI healthUI;

    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);

        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();  
        if (enemy)
        {
            TakeDamage(enemy.damage);
        }
    }

    private void TakeDamage(int damage){
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);
        //Flash Red
        StartCoroutine(FlashRed());


        if (currentHealth <= 0)
        {
           // player is die
        }

    }

    private IEnumerator FlashRed(){
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
