using System;
using System.Collections;
using Bounder.Framework;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMain : MonoBehaviour
{
    [Serializable]
    private enum EnemyType
    {
        Speedy,
        BigBoy,
        Regular
    }
    
    [Serializable]
    private struct EnemyTypeStats
    {
        public int maximumSpeed;
        public int acceleration;
        public int damage;
    }

    private Rigidbody2D _rb;
    private GameObject _player;
    private HealthComponent _hp;
    private SpriteRenderer _sr;
    private MaterialPropertyBlock _mpb;
    private bool dying;
    
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Collider2D col;
    
    [ShowOnEnum("enemyType", (int)EnemyType.Speedy)]
    [SerializeField] private EnemyTypeStats speedy;
    
    [ShowOnEnum("enemyType", (int)EnemyType.BigBoy)]
    [SerializeField] private EnemyTypeStats bigBoy;

    [ShowOnEnum("enemyType", (int)EnemyType.Regular)]
    [SerializeField] private EnemyTypeStats regular;

    private EnemyTypeStats stats;
    private float tookDamageTimer;
    private float tookDamageDuration = 1.0f;
    private float progress = 1.0f;
    private static readonly int Fade = Shader.PropertyToID("_Fade");
    private static readonly int HitFlash = Shader.PropertyToID("_HitFlash");

    private bool spriteFlipByDefault = false; // This is used, because we have some sprites that needed to be flipped by default
    
    // Setting up variables
    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        
        _rb = GetComponent<Rigidbody2D>();
        _hp = GetComponent<HealthComponent>();
        _sr = GetComponent<SpriteRenderer>();
        _mpb = new MaterialPropertyBlock();
        Physics2D.IgnoreCollision(col, _player.GetComponent<Collider2D>());
        
        
        switch (enemyType)
        {
            case EnemyType.Regular:
                stats = regular;
                break;
            case EnemyType.Speedy:
                stats = speedy;
                break;
            case EnemyType.BigBoy:
                stats = bigBoy;
                break;
        }

        spriteFlipByDefault = _sr.flipX;
        progress = 1;
    }
    
    private void OnEnable()
    {
        _hp.OnGameObjectHealthChanged.AddListener(OnGameObjectHealthChanged);
    }

    private void OnDisable()
    {
        _hp.OnGameObjectHealthChanged.RemoveListener(OnGameObjectHealthChanged);
    }

    private void OnGameObjectHealthChanged(GameObject instigator, HealthComponent healthcomp, float currenthealth, float actualdelta)
    {
        if (actualdelta < 0)
        {
            _rb.velocity = new Vector2(-_rb.velocity.x, 15); // Bounce enemy back
            StartCoroutine(HitFlashEffect());
        }

        if (currenthealth <= 0)
        {
            dying = true;
            GetComponent<Collider2D>().enabled = false;
            _rb.velocity = new Vector2(0, 0);
        }
    }

    void Update()
    {
        // If we are dying apply the material property block to the material, to get the fade effect happening, without duplicating the material
        if (dying)
        {
            _sr.GetPropertyBlock(_mpb);
            progress -= Time.deltaTime;
            _mpb.SetFloat(Fade, progress);
            _sr.SetPropertyBlock(_mpb, 0);
            
            if (progress <= 0)
                Destroy(gameObject);
        }
        
        // This timer is used for the bounce back effect, when an enemy has been hit
        if (tookDamageTimer > 0)
        {
            tookDamageTimer -= Time.deltaTime;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if dying or recently hit do not run the code
        if (dying || tookDamageTimer > 0) return;

        // get the X velocity based on which direction from the enemy, the player is
        float xVel = transform.position.x > _player.transform.position.x ? -stats.acceleration : stats.acceleration;

        // Add the acceleration to xVelocity
        xVel = Mathf.Clamp(xVel + _rb.velocity.x, -stats.maximumSpeed, stats.maximumSpeed);
        
        // apply velocity
        Vector2 newVelocity = new Vector2(xVel, _rb.velocity.y);
        _rb.velocity = newVelocity;

        // Flip Sprite based on movement direction
        if (_rb.velocity.x < 1)
        {
            _sr.flipX = !spriteFlipByDefault;
        }
        else
        {
            _sr.flipX = spriteFlipByDefault;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Apply Damage
        if (!other.CompareTag("Player")) return;
        
        HealthComponent hp = other.gameObject.GetComponent<HealthComponent>();
        if (hp)
        {
            hp.ApplyHealthChange(gameObject, -2);
        }
    }

    IEnumerator HitFlashEffect()
    {
        // Apply the hit flash effect and set it that we can not take damage during the effect
        float value = 0.0f;
        _hp.SetCanTakeDamage(false);
        tookDamageTimer = tookDamageDuration;
        
        
        while (value < 1)
        {
            value += Time.deltaTime * 7f;
            _sr.GetPropertyBlock(_mpb);
            _mpb.SetFloat(HitFlash, value);
            _sr.SetPropertyBlock(_mpb);
            yield return null;
        }

        while (value > 0)
        {
            value -= Time.deltaTime * 7f;
            _sr.GetPropertyBlock(_mpb);
            _mpb.SetFloat(HitFlash, value);
            _sr.SetPropertyBlock(_mpb);
            yield return null;
        }
        
        _hp.SetCanTakeDamage(true);
        
        yield return null;
    }
}
