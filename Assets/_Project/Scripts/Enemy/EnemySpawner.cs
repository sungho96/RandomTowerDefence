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
        if (!autoStart) return; //처음디버기용 테스트 & 만들었을때 제대로 동작하는 지 확인하기위해
        StartWave();
    }
    
    /// <summary>
    /// 루틴을 참고 하고
    /// SpawnRountine 중복적으로 생성하는것을 방지 하기 위한 방어 코드다.
    /// </summary>
    [ContextMenu("Start Wave")]
    public void StartWave()
    {
        if(routine != null)
            StopCoroutine(routine);//루틴을 참고 해서 루틴이 겹치면서 동작하지않도록 함.

        routine = StartCoroutine(SpawnRoutine());
    }


    private IEnumerator SpawnRoutine()
    {
        if (path == null || spawnPoint == null || enemyPrefab == null)// 기본적으로 방어코드를 선두에 작성해 트러블을 방지
        {
            Debug.LogError("[EnemySpawner] Missing refs : path/spawnPoint,enemyPrefab");
            yield break;
        }

        for ( int i =0; i<spawnCount; i++ )//테스트용으로 표현하였으나 캡슐화 및 접근성를 고려해 직렬화를 사용
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);//SerializeField(직렬화)를 사용해 인스펙터에서 수정할수있게끔 변경
        }
    }

    /// <summary>
    /// enemyprefab을 복제하는 함수로
    /// 복제를할때 생성위치,pos,rot등 복제하고
    /// 생성이되면 기본적인 정의(Init)를 대입한뒤
    /// 해당오브젝트를 활성화한다.
    /// </summary>
    [ContextMenu("Spawn one")]
    public void SpawnOne()
    {
        EnemyPathFollowes enemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            enemiesParent
        );
        enemy.Init(path);//waypoint.cs, transform.y 값 보정작업
        enemy.gameObject.SetActive(true);
    }

    /// <summary>
    /// Count= 몇마리의 적을 생성 수 
    /// interval= 연속으로 생성시 얼마의 텀을 줄지 시간(sec)
    /// 호출시 spawnOne을 호출하되 생성하면서도 GameState확인유무,GameOver를 상태를 계속확인하여
    /// 게임이끝나면 즉시 멈춤
    /// </summary>
    /// <param name="count"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public IEnumerator SpawnWave(int count, float interval)
    {
        if (path == null || spawnPoint == null || enemyPrefab == null) //계속해서 방어코드를 사용해 초기 에러를 진압하자
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

