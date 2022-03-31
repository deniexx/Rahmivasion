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

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float fHorizontalAcceleration = 2.0f;
    [SerializeField] private float fMaximumSpeed = 4.0f;

    
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<PlayerScript>().gameObject;
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
