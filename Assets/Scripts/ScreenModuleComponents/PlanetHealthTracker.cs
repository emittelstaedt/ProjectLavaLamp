using UnityEngine;
using UnityEngine.UI;

public class PlanetHealthTracker : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        Debug.Log("Planet health: " +  currentHealth);
        UpdateUI();

        if(currentHealth <= 0)
        {
            Debug.Log("Game Over");
        }
    }

    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}