using System;
using UnityEngine;

public class PassiveDamageComponent : MonoBehaviour
{
    [SerializeField] private float DamageToApply = -3.0f;
    [SerializeField] private int throwbackX = 5;
    [SerializeField] private int throwbackY = 5;

    // Apply Damage, and bounce the player in X and Y axis
    private void OnCollisionEnter2D(Collision2D col)
    {
        RahmivasionStaticLibrary.ApplyGameObjectHealthChange(this.gameObject, col.gameObject, DamageToApply);
        Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
        if (rb) rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * throwbackX, throwbackY);
    }
}
