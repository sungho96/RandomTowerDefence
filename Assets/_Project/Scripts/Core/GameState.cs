using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }// 싱글톤 활용
    [Header("Lives")]
    [SerializeField] private int maxLives = 20;
    [SerializeField] private int lives;
    [SerializeField] private int wave = 0;
    public int Wave => wave; //캡슐화 진행

    public int Lives => lives; 

    public event Action OnGameOver; //UI에서 사용하기위해 event Action 등록
    [SerializeField] private bool pauseGameover = true;
    public bool IsGameOver => lives <= 0; //Gameover lives 0이하 일경우 true

    private void Awake()
    {   
        //기존 instance 있으면 살리고 새로운 instance 삭제
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        lives = maxLives;
        Debug.Log($"[GameState] Lives init : {lives}/{maxLives}");
    }

    /// <summary>
    /// amount=lives를 감소시킬양
    /// Gameover일경우 일시정지 아닐경우
    ///lives 감소시키기
    ///만약 감소하고 0이될경우 게임 일시정지 함수 호출
    /// </summary>
    /// <param name="amount"></param>
    public void LossLife(int amount = 1)
    {
        if (IsGameOver) return;

        lives = Mathf.Max(0, lives - amount); //0미만 방어코드
        Debug.Log($"[GameStat] Lives : {lives}/{maxLives}");

        if (IsGameOver)
            TriggerGameOver();
    }
    /// <summary>
    /// 디버그로 게임일시정지 공유
    /// 업데이트 등 게임 일시정지후 OnGameover연결되어있는 호출 진행
    /// </summary>
    private void TriggerGameOver()
    {
        Debug.Log("[GameState] Game over");

        if (pauseGameover)
            Time.timeScale = 0f; //일시정지

        OnGameOver?.Invoke();
      
    }
    /// <summary>
    /// 호출되면 등록되어있는 Wave값을 변경하여 UI등 변경 제공
    /// </summary>
    /// <param name="value"></param>
    public void SetWave(int value)
    {
        wave = value;
        Debug.Log($"[GameState] Wave :{wave}");
    }
}
