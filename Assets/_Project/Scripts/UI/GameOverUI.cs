using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void OnEnable()
    {
        if (GameState.Instance != null)
            GameState.Instance.OnGameOver += Show;
    }

    private void OnDisable()
    {
        if (GameState.Instance != null)
            GameState.Instance.OnGameOver -= Show;
    }
    
    private void Show()
    {
        if (panel != null)
            panel.SetActive(true);
    }
}
