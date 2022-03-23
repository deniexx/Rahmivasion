using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveDamageComponent : MonoBehaviour
{
    [SerializeField] private float DamageToApply = -20.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RahmivasionStaticLibrary.ApplyGameObjectHealthChange(this.gameObject, collision.gameObject, DamageToApply);
    }
}
