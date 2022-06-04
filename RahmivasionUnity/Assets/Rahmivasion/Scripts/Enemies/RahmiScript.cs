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

    IEnumerator MoveLocation()
    {
        Vector3 startTransform = transform.position;

        float offset = Random.Range(-offsetX, offsetX);
        float progress = 0;

        while (progress < 1)
        {
            var position = player.transform.position;
            position.x += offset;
            position.y += offsetY;
            
            transform.position = Vector3.Lerp(startTransform, position, progress);
            progress += Time.deltaTime;
            yield return null;
        }
        
        yield return null;
        StartCoroutine(MoveLocation());
    }

    IEnumerator DropBomb()
    {
        float delayBetweenBombs = Random.Range(1.5f, 2.5f);

        while (delayBetweenBombs > 0)
        {
            delayBetweenBombs -= Time.deltaTime;
            if (delayBetweenBombs <= 0.0f)
            {
                laugh.Play();
                Instantiate(bomb, transform.position, bombRotation);
            }
            yield return null;
        }

        StartCoroutine(DropBomb());
        yield return null;
    }
}
