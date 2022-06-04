using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private AudioClip explosionSound;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Rahmi")) return;

        if (col.CompareTag("Player"))
            RahmivasionStaticLibrary.ApplyGameObjectHealthChange(this.gameObject, col.gameObject, -damage);
        
        AudioManager.GetInstance().PlaySoundEffect(explosionSound);
        
        Destroy(gameObject);
    }
}
