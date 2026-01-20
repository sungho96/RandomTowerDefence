using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private EnemySpawner spawner;

    [Header("Wave Settings")]
    [SerializeField] private int startWave = 1;
    [SerializeField] private int totalWaves = 5;

    [SerializeField] private int startSpawnCount = 5;
    [SerializeField] private int spawnCountIncreasePerWave = 2;

    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private float waveInterval = 3.0f;

    private Coroutine routine;

    private void Start()
    {
        if (spawner == null)
        {
            Debug.LogError("[WaveManger] Missing spawner reference.", this);
        }

        routine = StartCoroutine(WaveRoutine());
    }
    
    private IEnumerator WaveRoutine()
    {
        int wave = startWave;

        while (wave <= totalWaves)
        {
            if (GameState.Instance != null && GameState.Instance.IsGameOver)
                yield break;

            GameState.Instance?.SetWave(wave);

            int count = startSpawnCount + (wave - startWave) * spawnCountIncreasePerWave;

            yield return StartCoroutine(spawner.SpawnWave(count,spawnInterval));

            float t = 0f;
            while (t < waveInterval)
            {
                if (GameState.Instance != null && GameState.Instance.IsGameOver)
                    yield break;

                t += Time.deltaTime;
                yield return null;
            }

            wave++;
        }

        Debug.Log("[WaveManager] All waves Finished");
    }
    

}
