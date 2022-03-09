using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask lmWalls;
    [Header("Movement Variables")]
    [SerializeField] private float fJumpVelocity = 5f;
    [SerializeField] private float fJumpPressedRememberTime = 0.1f;
    [SerializeField] private float fGroundedRememberTime = 0.15f;
    [SerializeField] private float fHorizontalAcceleration = 0.25f;

    [Header("Damping and jump cut height")]
    [SerializeField] [Range(0, 1)] private float fHorizontalDampingBasic = 0.5f;
    [SerializeField] [Range(0, 1)] private float fHorizontalDampingWhenStopping = 0.7f;
    [SerializeField] [Range(0, 1)] private float fHorizontalDampingWhenTurning = 0.5f;
    [SerializeField] [Range(0, 1)] private float fJumpCutHeight = 0.5f;
    
    [SerializeField] private float fMaxSpeed = 4f;

    private Rigidbody2D _rb;

    private bool flipped;

    private float fJumpPressedRemember = 0;
    private float fGroundedRemember = 0f;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
        
    void Update()
    {
        Vector2 v2GroundedBoxCheckPosition = (Vector2) transform.position + new Vector2(0, -0.5f);
        Vector2 v2GroundedBoxCheckScale = (Vector2) transform.localScale + new Vector2(-0.02f, 0);
        bool bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);

        fGroundedRemember -= Time.deltaTime;
        if (bGrounded) fGroundedRemember = fGroundedRememberTime;
    
        fJumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump")) fJumpPressedRemember = fJumpPressedRememberTime;

        if (Input.GetButtonUp("Jump"))
        {
            if (_rb.velocity.y > 0)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * fJumpCutHeight);
            }
        }
               
    
        if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0))
        {
            fJumpPressedRemember = 0;
            fGroundedRemember = 0;
            _rb.velocity = new Vector2(_rb.velocity.x, fJumpVelocity);
        }
            
        float fHorizontalVelocity = _rb.velocity.x;
        float inputStrength = Input.GetAxisRaw("Horizontal");
        fHorizontalVelocity += inputStrength * fHorizontalAcceleration;
    
        if (Math.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
        else if ((Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(fHorizontalVelocity)))
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
        else
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);
    
        fHorizontalVelocity = Mathf.Clamp(fHorizontalVelocity, -fMaxSpeed, fMaxSpeed);
    
        AnimateCharacter(inputStrength);
            
        _rb.velocity = new Vector2(fHorizontalVelocity, _rb.velocity.y);
    
        if (Mathf.Abs(_rb.velocity.x) < 0.05)
        {
            _rb.velocity = new Vector2(0.0f, _rb.velocity.y);
        }
    }
    
    private void AnimateCharacter(float inputStrength)
    {
        if (inputStrength < -0.01f)
        {
            Flip(true);
        }
        else if (inputStrength > 0.01f)
        {
            Flip(false);
        }
    }
    
    private void Flip(bool flip)
    {
        transform.localScale = flip ? new Vector3(-1.0f, 1.0f, 1.0f) : new Vector3(1.0f, 1.0f, 1.0f);

        flipped = flip;
    }

    public bool GetIsPlayerFlipped()
    {
        return flipped;
    }
    
    public void StopPlayerInPlace()
    {
        _rb.velocity = new Vector2(0.0f, 0.0f);
    }
}
