using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMain : MonoBehaviour
{
    public enum EnemyType
    {
        Bouncer,
        Speedy,
        BigBoy
    }

    // Potentially create different scripts for each enemy type and add them as a component to the thingy
    [Serializable]
    private struct EnemyTypeStats
    {
        public int maximumSpeed;
        public int acceleration;
        public bool canJump;
        public int jumpChance;
        public int damage;
        public int jumpCD;
    }

    private Rigidbody2D _rb;
    private GameObject _player;
    private HealthComponent _hp;
    private SpriteRenderer _sr;
    private bool dying;

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float fHorizontalAcceleration = 2.0f;
    [SerializeField] private float fMaximumSpeed = 4.0f;
    [SerializeField] private EnemyTypeStats bouncer;
    [SerializeField] private EnemyTypeStats speedy;
    [SerializeField] private EnemyTypeStats bigBoy;

    private EnemyTypeStats stats;
    
    private static readonly int Fade = Shader.PropertyToID("_Fade");
    private static readonly int HitFlash = Shader.PropertyToID("_HitFlash");

    
    // Start is called before the first frame update
    void Awake()
    {
        _player = FindObjectOfType<PlayerScript>().gameObject;
        _rb = GetComponent<Rigidbody2D>();
        _hp = GetComponent<HealthComponent>();
        _sr = GetComponent<SpriteRenderer>();
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
            StartCoroutine(HitFlashEffect());
        
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
            float progress = _sr.material.GetFloat(Fade) - Time.deltaTime;
            _sr.material.SetFloat(Fade, progress);
            
            if (progress == 0)
                Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dying) return;
        
        float xVel = transform.position.x > _player.transform.position.x ? -fHorizontalAcceleration : fHorizontalAcceleration;

        xVel = Mathf.Clamp(xVel, -fMaximumSpeed, fMaximumSpeed);
        Vector2 newVelocity = new Vector2(xVel, _rb.velocity.y);
        _rb.velocity = newVelocity;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        HealthComponent hp = col.gameObject.GetComponent<HealthComponent>();
        if (hp)
        {
            hp.ApplyHealthChange(gameObject, -2);
        }
    }

    IEnumerator HitFlashEffect()
    {
        float value = _sr.material.GetFloat(HitFlash);
        _hp.SetCanTakeDamage(false);
        
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
        
        _hp.SetCanTakeDamage(true);
        
        yield return null;
    }
}
