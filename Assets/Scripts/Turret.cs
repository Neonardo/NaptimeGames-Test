using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Turret : MonoBehaviour
{
    [SerializeField] private Transform GunTransform;
    [SerializeField] private SpriteRenderer MainSprite;
    [SerializeField] private SpriteRenderer GunSprite;
    [SerializeField] private BoxCollider2D Collider;

    public bool IsAbleToSpawn => _lives > 0 ? true : false;
    public float SpawnTimeer => _spawnTimer;
    public bool IsActive => _active;

    public UnityEvent<Turret> OnRespawEvent = new UnityEvent<Turret>();
    public UnityEvent<Turret> OnDeathEvent = new UnityEvent<Turret>();

    const float TIME_BETWEEN_SHOTS = 1f;

    bool _active = false;
    int _lives = 3;
    float _rotateTimer = 0f;
    float _shootTimer = 0f;
    float _spawnTimer = 0f;

#region Unity
    private void Awake()
    {
        _active = true;
        _shootTimer = TIME_BETWEEN_SHOTS;
    }

    private void Update()
    {
        HandleTimers();

        if(_active)
            Shoot();
        else{
            if(_spawnTimer <= 0f)
            {
                Spawn();
            }
        }
    }
    private void FixedUpdate()
    {
        if(!_active) return;

        Rotate();
    }
#endregion

    public void Spawn()
    {
        OnRespawEvent.Invoke(this); // give information to GameManager, for it to handle random turret placement on the map
        _shootTimer = TIME_BETWEEN_SHOTS;
        Enable();
    }

    public void TakeDamage()
    {
        _lives--;
        
        if(_lives > 0)
        {
            _spawnTimer = 2f;
            Disable();
        }
        else{
            OnDeathEvent.Invoke(this); // inform GameManager to destroy and clean up this turret from the pool
        }
    }

    private void Disable()
    {
        _active = false;
        MainSprite.enabled = false;
        Collider.enabled = false;   
        GunSprite.enabled = false;
    }

    private void Enable()
    {
        MainSprite.enabled = true;
        Collider.enabled = true;
        GunSprite.enabled = true;
        _active = true;
    }

    private void Rotate()
    {
        if(_rotateTimer <= 0f)
        {
            var randomRotation = Random.Range(0, 361);
            transform.Rotate(Vector3.forward * randomRotation);

            _rotateTimer = Random.Range(0f, 1f);
        }
    }

    private void Shoot()
    {
        if(_shootTimer <= 0f)
        {
            // spawn projectile and let it handle it's logic
            var projectile = Persistent.Instance.ProjectilesPool.Get();
            projectile.GetComponent<Projectile>().Init(GunTransform.right);
            projectile.transform.position = GunTransform.position;
            projectile.SetActive(true);

            _shootTimer = TIME_BETWEEN_SHOTS;
        }
    }

    private void HandleTimers(){
        _rotateTimer -= Time.deltaTime;
        _shootTimer -= Time.deltaTime;
        _spawnTimer -= Time.deltaTime;
    }
}
