using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RahmiScript : MonoBehaviour
{
    private Transform player;

    [SerializeField] private int offsetX;
    [SerializeField] private int offsetY;
    [SerializeField] private GameObject bomb;
    [SerializeField] private AudioSource laugh;

    [SerializeField] private float minDelay = 2.0f;
    [SerializeField] private float maxDelay = 3.0f;
    private Quaternion bombRotation = new Quaternion(180.0f, 0.0f, 0.0f, 1.0f);

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(MoveLocation());
        StartCoroutine(DropBomb());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Move to a location that is close to the player
    IEnumerator MoveLocation()
    {
        Vector3 startTransform = transform.position;

        float offset = Random.Range(-offsetX, offsetX); // Choose a random offset on X
        float progress = 0;

        while (progress < 1)
        {
            // Get player position and the offsets and then lerp the enemy to the correct position
            var position = player.transform.position;
            position.x += offset;
            position.y += offsetY;
            
            transform.position = Vector3.Lerp(startTransform, position, progress);
            progress += Time.deltaTime;
            yield return null;
        }
        
        // After we are done moving call MoveLocation recursively
        StartCoroutine(MoveLocation());
        yield return null;
    }

    IEnumerator DropBomb()
    {
        float delayBetweenBombs = Random.Range(minDelay, maxDelay); // Choose a random delay

        while (delayBetweenBombs > 0)
        {
            delayBetweenBombs -= Time.deltaTime;
            if (delayBetweenBombs <= 0.0f) // Wait for when the delay has finished and then drop the bomb
            {
                laugh.Play();
                Instantiate(bomb, transform.position, bombRotation);
            }
            yield return null;
        }
        
        // After we are done dropping the bomb call DropBomb recursively
        StartCoroutine(DropBomb());
        yield return null;
    }
}
