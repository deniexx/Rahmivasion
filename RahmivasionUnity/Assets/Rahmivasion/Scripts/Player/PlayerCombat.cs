using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [HideInInspector]
    public int damage = 1;
    
    // Dead damage when collision has happened
    private void OnTriggerEnter2D(Collider2D col)
    {
        HealthComponent healthComp = RahmivasionStaticLibrary.GetHealthComponent(col.gameObject);
        
        if (healthComp != null)
            healthComp.ApplyHealthChange(this.gameObject, -damage);
    }
}
