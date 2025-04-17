using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth =3;
    private int currentHealth;
    public HealthUI healthUI;

    private SpriteRenderer spriteRenderer;
    
    public static event Action OnPlayerDied;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetHealth();

        spriteRenderer = GetComponent<SpriteRenderer>();
        GameController.OnReset += ResetHealth;
        HealthItem.OnHealthCollect += Heal;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();  
        if (enemy)
        {
            TakeDamage(enemy.damage);
        }
    }

    void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthUI.UpdateHearts(currentHealth);
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    private void TakeDamage(int damage){
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);
        //Flash Red
        StartCoroutine(FlashRed());


        if (currentHealth <= 0)
        {
           // player is die
           OnPlayerDied.Invoke();
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
