using UnityEngine;
using UnityEngine.SceneManagement;

public class Persistent : MonoBehaviour
{
    public static Persistent Instance;
    public int TurretsToSpawn => Instance._turrets;
    public Pool ProjectilesPool => _projectilesPool;

    private Pool _projectilesPool;
    private int _turrets = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Main")
        {
            var pools = FindObjectsOfType<Transform>();
            foreach (var pool in pools)
            {
                if(pool.gameObject.name.Contains("Projectiles"))
                {
                    _projectilesPool = pool.gameObject.GetComponent<Pool>();
                }
            }
        }
    }

    public void SetInfo(int turrets)
    {
        Instance._turrets = turrets;
    }
}
