using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }
    [Header("Lives")]
    [SerializeField] private int maxLives = 20;
    [SerializeField] private int lives;
    [SerializeField] private int wave = 0;
    public int Wave => wave;

    public int Lives => lives;

    public event Action OnGameOver;
    [SerializeField] private bool pauseGameover = true;
    public bool IsGameOver => lives <= 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        lives = maxLives;
        Debug.Log($"[GameState] Lives init : {lives}/{maxLives}");
    }

    public void LossLife(int amount = 1)
    {
        if (IsGameOver) return;

        lives = Mathf.Max(0, lives - amount);
        Debug.Log($"[GameStat] Lives : {lives}/{maxLives}");

        if (IsGameOver)
            TriggerGameOver();
    }

    [ContextMenu("Reset Lives")]
    public void ResetLives()
    {
        lives = maxLives;
        Debug.Log($"[GameState] Lives rest : {lives}/{maxLives}");
    }

    private void TriggerGameOver()
    {
        Debug.Log("[GameState] Game over");

        if (pauseGameover)
            Time.timeScale = 0f;

        OnGameOver?.Invoke();
      
    }
    public void SetWave(int value)
    {
        wave = value;
        Debug.Log($"[GameState] Wave :{wave}");
    }
}
