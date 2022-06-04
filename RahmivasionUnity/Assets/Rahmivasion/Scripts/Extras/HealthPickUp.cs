using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            RahmivasionStaticLibrary.GetHealthComponent(col.gameObject).ResetHealth();
    }
}
