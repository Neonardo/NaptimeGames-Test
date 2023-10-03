using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _speed = 20f;
    private bool _started = false;
    private Vector3 _direction = Vector3.zero;

    private float _lifeTime = 0f;

    public void Init(Vector3 dir)
    {
        _started = true;
        _direction = dir;
    }

    private void FixedUpdate()
    {
        if(_started)
        {
            transform.position += _direction * _speed * Time.fixedDeltaTime;
        }
    }

    private void Update()
    {
        if(_started) _lifeTime += Time.deltaTime;

        // if the projectile stays too long in the scene, and was not catched by the screen border collider, terminate it
        if(_lifeTime > 5f)
            Reset();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if( _lifeTime < 0.05f) return;

        if(other.gameObject.CompareTag("Turret"))
        {
            // handle turret damage
            other.gameObject.GetComponent<Turret>().TakeDamage();
            Reset();
        }
        // if the projectile hit out-of-screen border collider
        else if(other.gameObject.CompareTag("Border"))
        {
            Reset();
        }
    }

    private void Reset()
    {
        _started = false;
        _lifeTime = 0f;
        Persistent.Instance.ProjectilesPool.Return(gameObject);
    }
}
