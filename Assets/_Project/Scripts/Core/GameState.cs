using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }
    [Header("Lives")]
    [SerializeField] private int maxLives = 20;
    [SerializeField] private int lives;

    public int Lives => lives;
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
            Debug.Log("[GameState] Game over");
    }

    [ContextMenu("Reset Lives")]
    public void ResetLives()
    {
        lives = maxLives;
        Debug.Log($"[GameState] Lives rest : {lives}/{maxLives}");
    }
}
