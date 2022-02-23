using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D _rb;

    [SerializeField] private float maxSpeed = 4.0f;
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float jumpVelocity = -1.0f;
    [SerializeField] private float canJumpTime = 1.25f;

    private float jumpTime = 0.0f;

    private Vector2 newVelocity;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ProcessInput();
        
    }

    void ProcessInput()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        bool wantsToJump = Input.GetButtonDown("Jump");

        float delta = Time.deltaTime;

        float velocityX = Mathf.Clamp(horizontalAxis * speed, -maxSpeed, maxSpeed);
        float velocityY = _rb.velocity.y;


        if (wantsToJump && jumpTime <= 0.0f)
        {
            velocityY = jumpVelocity;
            jumpTime = canJumpTime;
        }

        jumpTime = Mathf.Clamp(jumpTime - delta, 0.0f, canJumpTime);

        _rb.velocity = new Vector2(velocityX, velocityY);

        if (horizontalAxis == 0)
        {
            _rb.velocity = Vector2.zero;
        }
    }
}
