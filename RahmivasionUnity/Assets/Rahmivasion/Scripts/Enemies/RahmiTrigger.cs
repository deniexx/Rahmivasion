using System;
using UnityEngine;

public class RahmiTrigger : MonoBehaviour
{
    [SerializeField] private GameObject rahmi;
    [SerializeField] private Transform spawnPosition;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Instantiate(rahmi, spawnPosition.position, spawnPosition.rotation);
            Destroy(gameObject);
        }
        
    }
}
