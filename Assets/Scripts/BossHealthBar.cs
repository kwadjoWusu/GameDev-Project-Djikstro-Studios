using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage;
    public Gradient healthGradient;
    
    private WorldCanvasPositioner canvasPositioner;
    
    private void Awake()
    {
        // Make sure we have a reference to the slider
        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }
        
        // Make sure we have a reference to the fill image
        if (fillImage == null && healthSlider != null)
        {
            fillImage = healthSlider.fillRect.GetComponent<Image>();
        }
        
        // Add WorldCanvasPositioner if not already present
        canvasPositioner = GetComponent<WorldCanvasPositioner>();
        if (canvasPositioner == null)
        {
            canvasPositioner = gameObject.AddComponent<WorldCanvasPositioner>();
        }
    }
    
    public void SetMaxHealth(int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            
            // Update color based on health percentage
            if (fillImage != null && healthGradient != null)
            {
                fillImage.color = healthGradient.Evaluate(1f);
            }
        }
    }
    
    public void UpdateHealth(int currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            
            // Update color based on health percentage
            if (fillImage != null && healthGradient != null)
            {
                float healthPercentage = (float)currentHealth / healthSlider.maxValue;
                fillImage.color = healthGradient.Evaluate(healthPercentage);
            }
        }
    }
    
    private void LateUpdate()
    {
        // The WorldCanvasPositioner component will handle keeping the canvas facing the camera
    }
}