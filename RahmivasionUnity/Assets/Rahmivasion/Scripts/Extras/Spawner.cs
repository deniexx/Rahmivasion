using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider2D))][ExecuteAlways]
public class Spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemiesToSpawn = new List<GameObject>();
    [SerializeField] private List<Transform> spawnLocations = new List<Transform>();
    [SerializeField] private float timeBetweenSpawns = 5.0f;
    [SerializeField] private List<GameObject> barriers = new List<GameObject>();

    private int enemiesKilled = 0;
    private bool activated = false;
    private int index = 0;
    private int previousSpawnIndex = 0;
    private List<GameObject> enemiesSpawned = new List<GameObject>();
    
    public void ResetSpawner()
    {
        activated = false;
        index = 0;
        enemiesKilled = 0;
        foreach (var enemy in enemiesSpawned)
        {
            RahmivasionStaticLibrary.KillGameObject(enemy);
        }
        SwitchBarriersState(false);
        enemiesSpawned.Clear();
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (activated) return;
        
        activated = true;
        SwitchBarriersState(true);
        StartCoroutine(SpawnEnemies());
    }

    private void OnHealthChanged(GameObject instigator, HealthComponent comp, float currentHealth, float delta)
    {
        if (currentHealth == 0)
        {
            enemiesSpawned.Remove(comp.gameObject);
            
            enemiesKilled++;
            
            if (enemiesKilled == enemiesToSpawn.Count)
            {
                SwitchBarriersState(false);
            }
        }
    }

    private void SwitchBarriersState(bool newState)
    {
        foreach (GameObject barrier in barriers)
        {
            barrier.SetActive(newState);
        }
    }
    
    IEnumerator SpawnEnemies()
    {
        int spawnIndex = Random.Range(0, spawnLocations.Count);
        if (spawnIndex == previousSpawnIndex)
            spawnIndex = spawnIndex + 1 < spawnLocations.Count ? spawnIndex + 1 : spawnIndex - 1;

        previousSpawnIndex = spawnIndex;
        GameObject enemy = Instantiate(enemiesToSpawn[index], spawnLocations[spawnIndex].position, spawnLocations[spawnIndex].rotation);
        enemy.GetComponent<HealthComponent>().OnGameObjectDamaged.AddListener(OnHealthChanged);
        enemiesSpawned.Add(enemy);
        index++;
        
        if (index < enemiesToSpawn.Count)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            StartCoroutine(SpawnEnemies());
        }
    }
}
