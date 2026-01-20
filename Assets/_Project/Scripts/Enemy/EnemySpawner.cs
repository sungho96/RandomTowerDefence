using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WayPointPath path;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform enemiesParent;

    [Header("Enemy Prefab (mush have EnemyPathFollower")]
    [SerializeField] private EnemyPathFollowes enemyPrefab;

    [Header("Wave Test")]
    [SerializeField] private int spawnCount = 5;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private bool autoStart = true;

    private Coroutine routine;

    private void Start()
    {
        if (!autoStart) return; ;
        StartWave();
    }
    [ContextMenu("Start Wave")]
    public void StartWave()
    {
        if(routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        if (path == null || spawnPoint == null || enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] Missing refs : path/spawnPoint,enemyPrefab");
            yield break;
        }

        for ( int i =0; i<spawnCount; i++ )
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    [ContextMenu("Spawn one")]
    public void SpawnOne()
    {
        EnemyPathFollowes enemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            enemiesParent
        );
        enemy.Init(path);
        enemy.gameObject.SetActive(true);
    }

    public IEnumerator SpawnWave(int count, float interval)
    {
        if (path == null || spawnPoint == null || enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] Mssing refs : path/spawnPoint/enemyprefab", this);
            yield break;
        }
        
        for (int i=0; i< count; i++)
        {
            if (GameState.Instance != null && GameState.Instance.IsGameOver)
                yield break;

            SpawnOne();
            yield return new WaitForSeconds(interval);
        }
        
    }
}

