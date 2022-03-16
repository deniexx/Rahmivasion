using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public delegate void OnGameObjectDamagedDelegate(GameObject instigator, HealthComponent healthComp, float currentHealth, float actualDelta);
    public OnGameObjectDamagedDelegate onGameObjectDamagedDelegate;

    [Header("Health Variables")]
    [SerializeField] private float baseHealth = 100.0f;
    [SerializeField] private float maxHealth = 100.0f;

    private float currentHealth = 0.0f;

    private void Start()
    {
        currentHealth = baseHealth;
    }

    public bool ApplyHealthChange(GameObject instigator, float delta)
    {
        float oldHealth = currentHealth;

        currentHealth = Mathf.Clamp(currentHealth + delta, 0.0f, maxHealth);
        float actualDelta = currentHealth - oldHealth;

        onGameObjectDamagedDelegate(instigator, this, currentHealth, actualDelta);

        return actualDelta != 0;
    }

    public void Kill()
    {
        ApplyHealthChange(this.gameObject, -maxHealth);
    }

    public bool IsAtMaxHealth()
    {
        return currentHealth == maxHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public bool IsAlive()
    {
        return currentHealth > 0.0f;
    }
}
