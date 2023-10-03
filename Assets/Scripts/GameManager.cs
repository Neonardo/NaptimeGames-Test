using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int TurretsToSpawn;
    [SerializeField] private Transform TurretsRoot;
    [SerializeField] private Transform ProjectilesRoot;

    [SerializeField] private GameObject TurretPrefab;
    [SerializeField] private GameObject ProjectilePrfeb;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private GameObject GameOverPanel;

    public static UnityEvent OnTowerDestroyed = new UnityEvent();

    private Pool _turretsPool;
    private Pool _projectilesPool;

    private List<Turret> _activeTurrets = new List<Turret>();

    private float _turretColliderSize = -1f;

    // camera
    float _halfCameraWidth = 0f;
    float _halfCameraHeight = 0f;

    private void Awake()
    {
        TurretsToSpawn = Persistent.Instance.TurretsToSpawn;

        _turretsPool = TurretsRoot.gameObject.GetComponent<Pool>();
        _turretsPool.Init(TurretsRoot, TurretPrefab, TurretsToSpawn);

        _projectilesPool = ProjectilesRoot.gameObject.GetComponent<Pool>();
        // allow small overhead of projectiles count compared to turrets, to minimise need to add new projectiles to pool during runtime
        _projectilesPool.Init(ProjectilesRoot, ProjectilePrfeb, (int)(TurretsToSpawn*1.25f)); 

        _turretColliderSize = Mathf.Max(
            TurretPrefab.GetComponent<BoxCollider2D>().size.x,
            TurretPrefab.GetComponent<BoxCollider2D>().size.y);

        SetupCamera();
    }

    private void Start()
    {
        for (int i = 0; i < TurretsToSpawn; i++)
        {
            TrySpawnNewTurret();
        }

        Debug.Log("Spawned " + _activeTurrets.Count + " out of " + TurretsToSpawn + " turrets");
    }

    private void GameOver()
    {
        GameOverPanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }

    public void TrySpawnNewTurret()
    {
        var spawned = false;
        var attempts = 0;

        while (!spawned && attempts < 500) // for tightly packed level allow lot of attempts to ensure every turret is placed
        {
            Vector2 randomPosition = GetRandomSpawnPosition();
            var canSpawn = CanSpawnAtPosition(randomPosition);

            if (canSpawn)
            {
                GameObject newTurret = _turretsPool.Get();
                var turretComp = newTurret.GetComponent<Turret>();
                _activeTurrets.Add(turretComp);

                // subscribe to events in the turret script that invoke on respawning and turret's destruction
                turretComp.OnRespawEvent.AddListener(TryRespawnTurret);
                turretComp.OnDeathEvent.AddListener(DestroyTurret);

                newTurret.transform.position = randomPosition;
                newTurret.gameObject.SetActive(true);
                spawned = true;
            }
            else
                attempts++;
        }
    }

    private void DestroyTurret(Turret turret)
    {
        _activeTurrets.Remove(turret);
        Destroy(turret.gameObject);

        if(_activeTurrets.Count == 1)
            GameOver();
    }

    public void TryRespawnTurret(Turret turret)
    {
        var spawned = false;
        var attempts = 0;

        while (!spawned && attempts < 500)
        {
            Vector2 randomPosition = GetRandomSpawnPosition();
            bool canSpawn = CanSpawnAtPosition(randomPosition);

            if (canSpawn)
            {
                turret.transform.position = randomPosition;
                turret.gameObject.SetActive(true);
                spawned = true;
            }
            else
                attempts++;
        }
    }

    private void SetupCamera()
    {
        float cameraSize = MainCamera.orthographicSize;

        float cameraWidth = cameraSize * 2f * MainCamera.aspect;
        float cameraHeight = cameraSize * 2f;

        // save camera calculations for usage in searching random location to spawn turrets
        _halfCameraWidth = cameraWidth / 2f;
        _halfCameraHeight = cameraHeight / 2f;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        // adjust placement range, taking into consideration size of a turret, to prevent turret spawning on the edge of the screen
        float randomX = Random.Range(-_halfCameraWidth + _turretColliderSize/2f, _halfCameraWidth - _turretColliderSize/2f);
        float randomY = Random.Range(-_halfCameraHeight + _turretColliderSize/2f, _halfCameraHeight - _turretColliderSize/2f);

        return new Vector2(randomX, randomY);
    }

    private bool CanSpawnAtPosition(Vector2 position)
    {
        // overlaping circle with small adjustment to the turret size to prevent turrets spawning too close to each other
        var colliders = Physics2D.OverlapCircleAll(position, _turretColliderSize * 1.15f);
        return colliders.Length == 0 || colliders == null;
    }
}
