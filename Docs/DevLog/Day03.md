# 📅 Day 3 — Enemy Wave Spawn + Path Inject(Init)까지 완료

> 오늘은 “게임 루프”의 뼈대 중 **스폰(웨이브) + 경로 주입(Init)**까지 완성했습니다.  
> 내일은 **Goal 도착 → Life 감소(GameState)**를 붙여서 “막아야 하는 이유”까지 만들 예정입니다.

---

## 🎯 오늘의 목표
- Enemy를 **SpawnPoint에서 일정 간격으로 여러 마리 생성**
- 생성된 Enemy가 **WayPoints 경로를 따라 이동**
- Inspector에서 수동 세팅이 아니라, **Init(path)로 런타임 경로 주입** 구조로 만들기

---

## ✅ 오늘 구현 결과
- `EnemySpawner`에서 `spawnCount`, `spawnInterval` 설정으로 웨이브 테스트 가능
- 스폰된 Enemy는 `Init(path)`를 통해 경로를 주입받고 즉시 이동 시작
- 이후 풀링(Object Pool)과 웨이브 확장에도 그대로 사용 가능한 구조 확보

---

## 🧠 오늘 사용한 Unity 기술(키워드)
- **Prefab / Instantiate**
  - 프리팹을 기반으로 런타임에 오브젝트 생성
- **[SerializeField]로 참조 주입(Reference 연결)**
  - `path`, `spawnPoint`, `enemyPrefab` 등을 Inspector에서 연결
- **Coroutine + WaitForSeconds**
  - 일정 간격으로 Enemy를 반복 생성 (웨이브 테스트)
- **Init 패턴(런타임 초기화)**
  - 스폰 직후 Enemy에 필요한 참조(path)를 넣고 상태를 리셋

---

## 🧩 EnemyPathFollwes.cs — Init 추가 (오늘 핵심 변경)

> Enemy가 Inspector에 의존하지 않고, 스폰 직후 `Init(path)`로 경로를 주입받도록 수정

```csharp
public void Init(WayPointPath newPath)
{
    path = newPath;
    currentIndex = 0;

    if (lockY)
        fixedY = transform.position.y;

    enabled = (path != null && path.Points != null && path.Points.Count > 0);
}
```

---

## 🧩 EnemySpawner.cs — 웨이브 스폰 구현 (오늘 작성한 전체 코드)

```csharp
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WayPointPath path;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform enemiesParent;

    [Header("Enemy Prefab (must have EnemyPathFollwes)")]
    [SerializeField] private EnemyPathFollwes enemyPrefab;

    [Header("Wave Test")]
    [SerializeField] private int spawnCount = 5;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private bool autoStart = true;

    private void Start()
    {
        if (!autoStart) return;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        if (path == null || spawnPoint == null || enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] Missing refs: path/spawnPoint/enemyPrefab", this);
            yield break;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    [ContextMenu("Spawn One")]
    public void SpawnOne()
    {
        EnemyPathFollwes enemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            enemiesParent
        );

        enemy.Init(path); // ✅ 스폰 직후 경로 주입
        enemy.gameObject.SetActive(true);
    }
}
```

---

## 🔧 Unity 세팅 체크 (오늘 완료한 Step4 기준)
1) Hierarchy에 `EnemySpawner` 오브젝트 생성 후 스크립트 부착  
2) Inspector에서 다음 참조 연결
   - `Path` → WayPoints(부모) 오브젝트 (=`WayPointPath`가 붙어있는 곳)
   - `SpawnPoint` → Start/Spawn 위치 Transform
   - `EnemyPrefab` → 공룡 Enemy 프리팹 (`EnemyPathFollwes`가 붙어있는 프리팹)
   - `EnemiesParent` → (선택) 런타임 생성 Enemy 정리용 부모

---

## ✅ 테스트 완료 기준 (오늘 기준)
- [x] Play 시 Enemy가 `spawnInterval` 간격으로 생성됨
- [x] 생성된 Enemy가 WayPoints를 따라 이동함
- [x] 스폰된 Enemy는 `path`를 Inspector가 아니라 `Init(path)`로 전달받아 동작함

---

## ⏭️ 내일 할 일 (Day 4로 넘어가기 전 “Day3 마무리”)
- Goal 도착 → Life 감소 붙이기
- `GameState` 생성 (Lives 관리)
- Enemy가 마지막 도착 시 `LoseLife()` 호출
- (추가) Lives가 0이면 GameOver 처리(로그/정지)

내일 목표는 **“막아야 하는 이유(라이프)”**를 붙여서 게임 루프를 완성하는 것.
