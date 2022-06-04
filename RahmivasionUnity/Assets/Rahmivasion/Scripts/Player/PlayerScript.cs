using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(HealthComponent))]
public class PlayerScript : MonoBehaviour
{
    public UnityEvent<GameObject> OnPlayerDead;
    
    [SerializeField] private LayerMask lmWalls;
    [Header("Movement Variables")]
    [SerializeField] private float fJumpVelocity = 14f;
    [SerializeField] private float fJumpPressedRememberTime = 0.1f;
    [SerializeField] private float fGroundedRememberTime = 0.15f;
    [SerializeField] private float fHorizontalAcceleration = 0.25f;

    [Space(20)]
    [Header("Damping and jump cut height")]
    [SerializeField] [Range(0, 1)] private float fHorizontalDampingBasic = 0.5f;
    [SerializeField] [Range(0, 1)] private float fJumpCutHeight = 0.5f;

    [SerializeField] private float fMaxSpeed = 4f;

    private bool dead = false;
    private Touch currentTouch;

    private Rigidbody2D _rb;
    private PlayerCombat _pc;
    private HealthComponent _healthComp;
    private SpriteRenderer _sr;
    private Animator _animator;

    // Start of touch stuff
    Vector2 startTouchPos;
    Vector2 endTouchPos;

    DateTime touchStartTime;

    DateTime touchEndTime;

    bool touchStarted = false;

    [Space(20)]
    [Header("Swiping Controls")]
    [SerializeField] float swipeTimeThreshold = 0.1f;
    [SerializeField] float swipeLengthThreshold = 0.1f;
    [SerializeField] private bool isUsingSwipe = false;
    
    // Ending of touch stuff
    
    [Space(20)]
    [Header("Combat")]
    [SerializeField] private int damage = 3;
    [SerializeField] private float defaultAttackCooldown = 0.75f;
    [SerializeField] private float invulnerabilityDuration = 2.0f;
    [SerializeField] private Collider2D col;
    private float invulnerabilityTimer;

    private float attackCooldown = 0.0f;

    private float autoRunStrength = 0;
    
    // Arena variables
    private float healthOnArenaStart = 0;
    private float damageTakenWithinArena = 0;
    private bool inArena = false;


    [Space(20)] 
    [Header("Combat Sounds")]
    [SerializeField] private AudioClip someDamageTaken; // when the player takes 20% or less of his health as damage
    [SerializeField] private AudioClip mediumAmountOfDamageTaken; // when the player takes 50% or less of his health as damage
    [SerializeField] private AudioClip lotsOfDamageTaken; // when the player takes 80% or less of his health as damage
    [SerializeField] private AudioClip noDamageTaken; // when the player has taken no damage
    [SerializeField] private AudioClip arenaLost; // when the player has died during the arena
    // End of arena variables

    [Space(20)] 
    [Header("Sounds")] 
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip hit; // Sound to be played when the player has been hit
    [SerializeField] private AudioClip attack; // Sound for when the player attacks

    [SerializeField] private TextMeshProUGUI health;

    private bool frozen;

    private float inputStrength;
    private float fJumpPressedRemember;
    private float fGroundedRemember;
    private static readonly int Fade = Shader.PropertyToID("_Fade");
    private static readonly int HitFlash = Shader.PropertyToID("_HitFlash");
    private static readonly int HorizontalVelocity = Animator.StringToHash("_HorizontalVelocity");
    private static readonly int Attack = Animator.StringToHash("_Attack");
    private static readonly int Jump = Animator.StringToHash("_Jump");

    /// <summary>
    /// A Unity event used to set up the player variables and set the collision to ignore the weapon collider, so we do not deal damage to ourselves
    /// </summary>
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _healthComp = RahmivasionStaticLibrary.GetHealthComponent(this.gameObject);
        _sr = GetComponentInChildren<SpriteRenderer>();
        _pc = GetComponentInChildren<PlayerCombat>();
        _animator = GetComponent<Animator>();
        isUsingSwipe = GameManager.GetInstance().GetIsUsingSwipeInput();
        
        col.enabled = false;
        
        // Ignoring collision with weapon
        Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
    }

    /// <summary>
    /// Setting up variables that we need to be done after Awake method to avoid issues
    /// </summary>
    private void Start()
    {
        _pc.damage = damage;
        _sr.sharedMaterial.SetFloat(HitFlash, 0);
        health.text = _healthComp.GetHealth().ToString();
        StartCoroutine(MakeVisible(1));
    }
    
    private void OnEnable()
    {
        _healthComp.OnGameObjectHealthChanged.AddListener(OnHealthChanged);
    }

    private void OnDisable()
    {
        _healthComp.OnGameObjectHealthChanged.RemoveListener(OnHealthChanged);
    }
    
    void Update()
    {
        // If the player is not dead run the update code
        if (!dead)
        {
            // Process Movement
            ProcessMovement();
            
            if (Input.GetKeyDown(KeyCode.F)) TryAttack(); // Used to debug
            
            // Cooldown between attacks
            if (attackCooldown > 0.0f)
            {
                attackCooldown -= Time.deltaTime;
            }

            // Invulnerability time
            if (invulnerabilityTimer > 0.0f)
            {
                invulnerabilityTimer -= Time.deltaTime;
                if (invulnerabilityTimer <= 0.0f) _healthComp.SetCanTakeDamage(true);
            }
        }
    }
    
    /// <summary>
    /// Tries to attack if the attack cooldown has expired
    /// </summary>
    public void TryAttack()
    {
        if (attackCooldown > 0.0f) return;

        PlaySoundClip(attack);
        attackCooldown = defaultAttackCooldown;
        _animator.SetTrigger(Attack);
        
    }
    
    private void ProcessMovement()
    {
        if (frozen) return;

        // Getting the player transform and projecting a BoxCollider to check if the player overlaps with the layer for ground/walls and storing that in bGrounded
        var pTransform = transform;
        Vector2 v2GroundedBoxCheckPosition = (Vector2) pTransform.position + new Vector2(0, -0.5f);
        Vector2 v2GroundedBoxCheckScale = (Vector2) pTransform.localScale + new Vector2(-0.02f, 0);
        bool bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);

        // Remembering if we have been grounded, used for the coyote time
        fGroundedRemember -= Time.deltaTime;
        if (bGrounded) fGroundedRemember = fGroundedRememberTime;
    
        // Remembers if we have pressed the jump button, allows for jumping even if we pressed a bit before touching the ground
        fJumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump")) fJumpPressedRemember = fJumpPressedRememberTime; // Used to debug

        // Used for debugging, reducing the upwards velocity, when the key has been released
        if (Input.GetButtonUp("Jump"))
        {
            if (_rb.velocity.y > 0)
            {
                var velocity = _rb.velocity;
                velocity = new Vector2(velocity.x, velocity.y * fJumpCutHeight);
                _rb.velocity = velocity;
            }
        }

        // Checks if we are using swipe input and if so, we set up the inputStrength to always be -1, 0 or 1
        if (isUsingSwipe) inputStrength = autoRunStrength;
        
        // If we remember being grounded and we remember that jump key is pressed jump
        if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0))
        {
            fJumpPressedRemember = 0;
            fGroundedRemember = 0;
            _rb.velocity = new Vector2(_rb.velocity.x, fJumpVelocity);
        }
        
        float fHorizontalVelocity = _rb.velocity.x;

        ProcessSwipes();

        // Applies horizontal velocity based on direction we are moving
        fHorizontalVelocity += inputStrength * fHorizontalAcceleration * Time.deltaTime;
        
        // Used for slowing down
        fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);
        
        fHorizontalVelocity = Mathf.Clamp(fHorizontalVelocity, -fMaxSpeed, fMaxSpeed);
    
        AnimateCharacter();
            
        _rb.velocity = new Vector2(fHorizontalVelocity, _rb.velocity.y);
        _animator.SetFloat(HorizontalVelocity, Mathf.Abs(_rb.velocity.x));
    
        // If we are moving very slowly on the X axis, stop and only jump upwards, used for more consistent behaviour when avoiding spikes
        if (Mathf.Abs(_rb.velocity.x) < 0.05)
        {
            _rb.velocity = new Vector2(0.0f, _rb.velocity.y);
        }
    }

    private void ProcessSwipes()
    {
        // Sets up variables when we start dragging the mouse
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = Input.mousePosition;
            endTouchPos = Input.mousePosition;
            touchStartTime = DateTime.Now;
            touchStarted = true;
        }
        
        // Checks the swipe after we release the mouse button
        if (Input.GetMouseButtonUp(0) && touchStarted)
        {
            endTouchPos = Input.mousePosition;
            touchEndTime = DateTime.Now;
            CheckSwipe();
        }

        // Same as above but with touch input
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    startTouchPos = touch.position;
                    endTouchPos = touch.position;
                    touchStartTime = DateTime.Now;
                    touchStarted = true;
                }

                else if (touch.phase == TouchPhase.Ended)
                {
                    endTouchPos = touch.position;
                    touchEndTime = DateTime.Now;
                    CheckSwipe();
                }
            }
        }
    }

    private void CheckSwipe()
    {
        // Get how long did it take us to swipe, used to determine whether swipe was intended or only click
        float duration = (float)touchEndTime.Subtract(touchStartTime).TotalSeconds;
    
        // Get the direction of the swipe
        Vector2 direction = endTouchPos - startTouchPos;

        if (duration < swipeTimeThreshold) return;
        if (direction.magnitude < swipeLengthThreshold) return;

        // Evaluate the swipe direction and apply movement input in appropriate direction
        if (direction.x < -100)
        {
            autoRunStrength = -1; // Move left
        }
        else if (direction.x > 100)
        {
            autoRunStrength = 1; // Move Right
        }
        if (direction.y < -100)
        {
            autoRunStrength = 0; // Stop Moving
        }
        else if (direction.y > 100)
        {
            JumpFromButton(); // Jump
        }
    }

    private void OnHealthChanged(GameObject instigator, HealthComponent healthComp, float currentHealth, float actualDelta)
    {
        // Set up the health UI text
        health.text = currentHealth.ToString();
        
        // If damage has been taken
        if (actualDelta < 0)
        {
            StartCoroutine(HitFlashEffect());
            PlaySoundClip(hit);
            
            if (inArena)
                damageTakenWithinArena += actualDelta;
        }
        
        // If we are dead
        if (currentHealth == 0)
        {
            StopPlayerInPlace();
            dead = true;
            StartCoroutine(MakeVisible(-1));
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
    }

    private void ResetPlayer()
    {
        _sr.sharedMaterial.SetFloat(HitFlash, 0);
        _healthComp.ResetHealth();
        GameManager.GetInstance().RespawnPlayer(gameObject);
        StopAllCoroutines();
        StartCoroutine(MakeVisible(1));
    }

    private void StopPlayerInPlace()
    {
        _rb.velocity = new Vector2(0.0f, 0.0f);
    }

    private void PlaySoundClip(AudioClip clip)
    {
        playerAudioSource.Stop();
        playerAudioSource.clip = clip;
        playerAudioSource.Play();
    }

    // Input from the on screen buttons
    public void JumpFromButton()
    {
        _animator.SetTrigger(Jump);
        fJumpPressedRemember = fJumpPressedRememberTime;
    }

    // Used for when we release the jump button on screen to adjust Y velocity, that way we have an adaptable jump to hold long you hold it
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

    /// <summary>
    /// Set if the player is Frozen, i.e. Can not move
    /// </summary>
    /// <param name="newState">Boolean, the new state, true - can move, false - can not</param>
    public void SetFrozen(bool newState)
    {
        if (newState == true)
        {
            _rb.velocity = Vector2.zero;
            frozen = true;
        }
        else
        {
            frozen = false;    
        }
    }

    public void ArenaStarted()
    {
        inArena = true;
        healthOnArenaStart = _healthComp.GetHealth();
        damageTakenWithinArena = 0;
    }

    public void ArenaFinished(bool arenaWon)
    {
        // Check which sound to play based on the outcome of the arena
        if (arenaWon)
        {
            float healthDelta = damageTakenWithinArena / healthOnArenaStart;
            
            
            if (healthDelta > 0.79f)
                PlaySoundClip(lotsOfDamageTaken);
            else if (healthDelta > 0.49f)
                PlaySoundClip(mediumAmountOfDamageTaken);
            else if (healthDelta > 0)
                PlaySoundClip(someDamageTaken);
            else
                PlaySoundClip(noDamageTaken);
        }
        else
            PlaySoundClip(arenaLost);
    }

    IEnumerator MakeVisible(int dir)
    {
        // Makes the player visible through the shader
        float progress = dir > 0 ? 0.0f : 1.0f;

        // Freezes the player while the effect is happening
        frozen = true;
        
        if (dir > 0)
        {
            while (progress < 1.0f)
            {
                progress = _sr.sharedMaterial.GetFloat(Fade) + (Time.deltaTime * dir);
                _sr.sharedMaterial.SetFloat(Fade, progress);
                yield return null;
            }
        }

        else
        {
            while (progress > 0.0f)
            {
                progress = _sr.sharedMaterial.GetFloat(Fade) + (Time.deltaTime * dir * 0.5f);
                _sr.sharedMaterial.SetFloat(Fade, progress);
                yield return null;
            }
            OnPlayerDead?.Invoke(this.gameObject);
            ResetPlayer();
        }

        // Unfreeze player if we have been made visible
        if (progress > 0.5f)
        {
            frozen = false;
            dead = false;
        }
        
        yield return null;
    }

    IEnumerator HitFlashEffect()
    {
        // Apply the hit flash effect to the player and then go back to normal
        
        float value = _sr.sharedMaterial.GetFloat(HitFlash);
        _healthComp.SetCanTakeDamage(false);
        invulnerabilityTimer = invulnerabilityDuration;

        while (value < 1)
        {
            value += Time.deltaTime * 7f;
            _sr.sharedMaterial.SetFloat(HitFlash, value);
            yield return null;
        }

        while (value > 0)
        {
            value -= Time.deltaTime * 7f;
            _sr.sharedMaterial.SetFloat(HitFlash, value);
            yield return null;
        }
        
        yield return null;
    }
}
