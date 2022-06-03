using System;
using System.Collections;
using Bounder.Framework;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMain : MonoBehaviour
{
    public enum EnemyType
    {
        Speedy,
        BigBoy,
        Regular
    }

    // Potentially create different scripts for each enemy type and add them as a component to the thingy
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

    private bool spriteFlipByDefault = false;

    
    // Start is called before the first frame update
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
        _hp.OnGameObjectDamaged.AddListener(OnGameObjectDamaged);
    }

    private void OnDisable()
    {
        _hp.OnGameObjectDamaged.RemoveListener(OnGameObjectDamaged);
    }

    private void OnGameObjectDamaged(GameObject instigator, HealthComponent healthcomp, float currenthealth, float actualdelta)
    {
        if (actualdelta < 0)
        {
            _rb.velocity = new Vector2(-_rb.velocity.x, 15);
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
        if (dying)
        {
            _sr.GetPropertyBlock(_mpb);
            progress -= Time.deltaTime;
            _mpb.SetFloat(Fade, progress);
            _sr.SetPropertyBlock(_mpb, 0);
            
            if (progress <= 0)
                Destroy(gameObject);
        }
        
        
        if (tookDamageTimer > 0)
        {
            tookDamageTimer -= Time.deltaTime;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dying || tookDamageTimer > 0) return;

        float xVel = transform.position.x > _player.transform.position.x ? -stats.acceleration : stats.acceleration;

        xVel = Mathf.Clamp(xVel + _rb.velocity.x, -stats.maximumSpeed, stats.maximumSpeed);
        Vector2 newVelocity = new Vector2(xVel, _rb.velocity.y);
        _rb.velocity = newVelocity;

        if (_rb.velocity.x < 1)
        {
            _sr.flipX = !spriteFlipByDefault;
        }
        else
        {
            _sr.flipX = spriteFlipByDefault;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        
        HealthComponent hp = col.gameObject.GetComponent<HealthComponent>();
        if (hp)
        {
            hp.ApplyHealthChange(gameObject, -2);
        }
    }

    IEnumerator HitFlashEffect()
    {
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
