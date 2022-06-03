using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private List<Spawner> spawners;
    
    void Awake()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().OnPlayerDead.AddListener(OnPlayerDead);
    }

    private void OnPlayerDead(GameObject player)
    {
        foreach (Spawner spawner in spawners)
        {
            spawner.StopAllCoroutines();
            spawner.ResetSpawner();
        }
    }
}
