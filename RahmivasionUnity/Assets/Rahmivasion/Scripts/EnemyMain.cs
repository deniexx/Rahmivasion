using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain : MonoBehaviour
{
    private Rigidbody2D _rb;
    private GameObject _player;

    [SerializeField] private float fHorizontalAcceleration = 2.0f;
    [SerializeField] private float fMaximumSpeed = 4.0f;

    
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<PlayerMovement>().gameObject;
        _rb = GetComponent<Rigidbody2D>();       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float xVel = transform.position.x > _player.transform.position.x ? -fHorizontalAcceleration : fHorizontalAcceleration;

        xVel = Mathf.Clamp(xVel, -fMaximumSpeed, fMaximumSpeed);
        Vector2 newVelocity = new Vector2(xVel, _rb.velocity.y);
        _rb.velocity = newVelocity;

    }
}
