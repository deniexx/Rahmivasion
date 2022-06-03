using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class HealthComponent : MonoBehaviour
{
    public UnityEvent<GameObject, HealthComponent, float, float> OnGameObjectDamaged;

    [Header("Health Variables")]
    [SerializeField] private float baseHealth = 6.0f;
    [SerializeField] private float maxHealth = 6.0f;
    private bool canTakeDamage = true;
    
    private float currentHealth = 0.0f;

    private void Start()
    {
        currentHealth = baseHealth;
    }
    
    /// <summary>
    /// Applies change to health based on delta
    /// </summary>
    /// <param name="instigator">This is the gameObject that has called this event</param>
    /// <param name="delta">The difference in health to be applied, should be negative number if you want to apply damage, default value is 1</param>
    /// <returns>Returns true if there was a change in the health of the player</returns>
    public bool ApplyHealthChange(GameObject instigator, float delta = 1)
    {
        float oldHealth = currentHealth;

        currentHealth = Mathf.Clamp(currentHealth + delta, 0.0f, maxHealth);
        if (!canTakeDamage && currentHealth - oldHealth < 0)
        {
            currentHealth = oldHealth;
            return false;
        }
        
        float actualDelta = currentHealth - oldHealth;
        
        OnGameObjectDamaged?.Invoke(instigator, this, currentHealth, actualDelta);

        return actualDelta != 0;
    }

    /// <summary>
    /// Sets the can take damage variable
    /// </summary>
    /// <param name="newValue">Can be TRUE or FALSE, whether the player can take damage or not</param>
    public void SetCanTakeDamage(bool newValue)
    {
        canTakeDamage = newValue;
    }

    /// <summary>
    /// Resets the current health to be equal to the baseHealth
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = baseHealth;
    }

    /// <summary>
    /// Kills the gameObject, sets it's health to 0
    /// </summary>
    /// <returns></returns>
    public void Kill()
    {
        ApplyHealthChange(this.gameObject, -maxHealth);
    }
    
    /// <summary>
    /// Checks if the gameObjects Health is max
    /// </summary>
    /// <returns>True if player is at max health and false if he's not</returns>
    public bool IsAtMaxHealth()
    {
        return currentHealth == maxHealth;
    }

    /// <summary>
    /// Returns the max health of the player
    /// </summary>
    /// <returns>Float value of the gameObject's MaxHealth</returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    /// <summary>
    /// Gets the player of the health
    /// </summary>
    /// <returns>Float value of the gameObject's health</returns>
    public float GetHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Checks if gameObject's health is more than 0
    /// </summary>
    /// <returns>True if gameObject's health is more than 0, else false</returns>
    public bool IsAlive()
    {
        return currentHealth > 0.0f;
    }
}
