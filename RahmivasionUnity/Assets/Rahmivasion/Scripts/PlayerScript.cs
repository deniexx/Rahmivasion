using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(HealthComponent))]
public class PlayerScript : MonoBehaviour
{
    public delegate void OnPlayerDead(GameObject player);
    public OnPlayerDead onPlayerDead;

    [SerializeField] private LayerMask lmWalls;
    [Header("Movement Variables")]
    [SerializeField] private float fJumpVelocity = 14f;
    [SerializeField] private float fJumpPressedRememberTime = 0.1f;
    [SerializeField] private float fGroundedRememberTime = 0.15f;
    [SerializeField] private float fHorizontalAcceleration = 0.25f;

    [Header("Damping and jump cut height")]
    [SerializeField] [Range(0, 1)] private float fHorizontalDampingBasic = 0.5f;
    [SerializeField] [Range(0, 1)] private float fHorizontalDampingWhenStopping = 0.7f;
    [SerializeField] [Range(0, 1)] private float fHorizontalDampingWhenTurning = 0.5f;
    [SerializeField] [Range(0, 1)] private float fJumpCutHeight = 0.5f;

    [SerializeField] private float fMaxSpeed = 4f;

    private bool dead = false;
    private Touch currentTouch;

    private Rigidbody2D _rb;
    private HealthComponent _healthComp;
    private SpriteRenderer _sr;

    private bool flipped;

    private float inputStrength;
    private float fJumpPressedRemember;
    private float fGroundedRemember;
    private static readonly int Fade = Shader.PropertyToID("_Fade");
    private static readonly int HitFlash = Shader.PropertyToID("_HitFlash");

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _healthComp = RahmivasionStaticLibrary.GetHealthComponent(this.gameObject);
        _sr = GetComponentInChildren<SpriteRenderer>();
        /*
        forwardTouchPosition = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("ForwardButton").transform.position);
        backwardTouchPosition = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("BackwardsButton").transform.position);
        */
    }

    private void OnEnable()
    {
        _healthComp.onGameObjectDamagedDelegate += OnHealthChanged;
    }

    private void OnDisable()
    {
        _healthComp.onGameObjectDamagedDelegate -= OnHealthChanged;
    }

    void Update()
    {
        if (!dead)
        {
            ProcessMovement();
        }
        else
        {
            float progress = _sr.material.GetFloat(Fade) - Time.deltaTime;
            _sr.material.SetFloat(Fade, progress);
        }
    }
    
    private void ProcessMovement()
    {
        var pTransform = transform;
        Vector2 v2GroundedBoxCheckPosition = (Vector2) pTransform.position + new Vector2(0, -0.5f);
        Vector2 v2GroundedBoxCheckScale = (Vector2) pTransform.localScale + new Vector2(-0.02f, 0);
        bool bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);

        fGroundedRemember -= Time.deltaTime;
        if (bGrounded) fGroundedRemember = fGroundedRememberTime;
    
        fJumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump")) fJumpPressedRemember = fJumpPressedRememberTime;

        if (Input.GetButtonUp("Jump"))
        {
            if (_rb.velocity.y > 0)
            {
                var velocity = _rb.velocity;
                velocity = new Vector2(velocity.x, velocity.y * fJumpCutHeight);
                _rb.velocity = velocity;
            }
        }
               
    
        if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0))
        {
            fJumpPressedRemember = 0;
            fGroundedRemember = 0;
            _rb.velocity = new Vector2(_rb.velocity.x, fJumpVelocity);
        }
            
        float fHorizontalVelocity = _rb.velocity.x;

        //inputStrength = Input.GetAxisRaw("Horizontal");
        
        if (Input.touchCount > 0)
        {
            /*
            for (int i = 0; i < Input.touchCount; ++i)
            {
                currentTouch = Input.GetTouch(i);
                
                if (currentTouch.position == forwardTouchPosition)
                    inputStrength = 1;
                else if (currentTouch.position == backwardTouchPosition)
                    inputStrength = -1;
                else inputStrength = 0;
            }
            */
        }
        
        fHorizontalVelocity += inputStrength * fHorizontalAcceleration * Time.deltaTime;
    
        if (Math.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
        else if ((Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(fHorizontalVelocity)))
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
        else
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

        fHorizontalVelocity = Mathf.Clamp(fHorizontalVelocity, -fMaxSpeed, fMaxSpeed);
    
        AnimateCharacter();
            
        _rb.velocity = new Vector2(fHorizontalVelocity, _rb.velocity.y);
    
        if (Mathf.Abs(_rb.velocity.x) < 0.05)
        {
            _rb.velocity = new Vector2(0.0f, _rb.velocity.y);
        }
    }

    private void OnHealthChanged(GameObject instigator, HealthComponent healthComp, float currentHealth, float actualDelta)
    {
        // @TODO: Implement event to flash the player when he has been damaged and stuff
        Debug.Log($"Health has been changed. New Health is {currentHealth}");

        if (actualDelta < 0)
        {
            StartCoroutine(HitFlashEffect());
        }

        if (currentHealth == 0)
        {
            //onPlayerDead(this.gameObject);
            StopPlayerInPlace();
            dead = true;
        }
    }
    
    private void AnimateCharacter()
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

    // Input from the on screen buttons
    public void JumpFromButton()
    {
        fJumpPressedRemember = fJumpPressedRememberTime;
    }

    public void JumpButtonReleased()
    {
        if (_rb.velocity.y > 0)
        {
            var velocity = _rb.velocity;
            velocity = new Vector2(velocity.x, velocity.y * fJumpCutHeight);
            _rb.velocity = velocity;
        }
    }

    public void SetInputStrength(float value)
    {
        inputStrength = value;
    }

    public void ResetInputStrength()
    {
        inputStrength = 0.0f;
    }

    IEnumerator HitFlashEffect()
    {
        float value = _sr.material.GetFloat(HitFlash);

        while (value < 1)
        {
            value += Time.deltaTime * 7f;
            _sr.material.SetFloat(HitFlash, value);
            yield return null;
        }

        while (value > 0)
        {
            value -= Time.deltaTime * 7f;
            _sr.material.SetFloat(HitFlash, value);
            yield return null;
        }
        
        yield return null;
    }
}
