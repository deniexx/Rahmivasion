using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveDamageComponent : MonoBehaviour
{
    [SerializeField] private float DamageToApply = 20.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RahmivasionStaticLibrary.GetHealthComponent(collision.gameObject).ApplyHealthChange(this.gameObject, -DamageToApply);
    }
}
