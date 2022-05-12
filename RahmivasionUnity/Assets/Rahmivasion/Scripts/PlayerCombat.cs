using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private int damage = 3;
    [SerializeField] private float defaultAttackCooldown = 0.25f;
    public GameObject weapon;
    [SerializeField] private bool canReset = false;
    
    private float attackCooldown = 0.0f;
    private Animation _anim;
    private Collider2D col;

    private void Awake()
    {
        _anim = weapon.GetComponent<Animation>();
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void Attack()
    {
        if (attackCooldown > 0.0f) return;
        
        attackCooldown = defaultAttackCooldown;
        col.enabled = true;
        _anim.Play();
        //GetComponent<Animator>().SetBool("Attack", true);
    }
    
    private void Update()
    {
        if (attackCooldown > 0.0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (canReset)
        {
            weapon.transform.rotation = Quaternion.identity;
            col.enabled = false;
            canReset = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        HealthComponent healthComp = RahmivasionStaticLibrary.GetHealthComponent(col.gameObject);
        
        if (healthComp != null)
            healthComp.ApplyHealthChange(this.gameObject, -damage);
    }
}
