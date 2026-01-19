# 📅 Day 4 — Goal 도착 → Life 감소(GameState) + HUD(LivesText) 연결

> 오늘은 “막아야 하는 이유”를 완성했습니다.  
> Enemy가 Goal에 도착하면 **Life가 감소**하고, 그 값이 **HUD(TextMeshPro)** 에 바로 반영되도록 연결했습니다.

---

## 🎯 오늘의 목표
- Enemy가 Goal에 도착했을 때 **Life 감소**
- Life 상태를 관리하는 **GameState 싱글톤** 구현
- 화면에 **LivesText(HUD)** 로 표시
- UI는 **Anchor Preset**으로 원하는 위치(예: 좌상단)에 고정

---

## ✅ 오늘 구현 결과
- `GameState`에서 `Lives`, `IsGameOver` 관리 가능
- Enemy가 Goal에 닿으면 `GameState.Instance.LoseLife()` 호출
- `LivesText(TMP)`가 GameState 값을 표시하도록 연결
- HUD 앵커 프리셋 이동으로 “원하는 UI 위치 고정” 성공

---

## 🧱 Hierarchy 구성 (정확한 위치)

### ✅ UI
- `Main`
  - `UI`
    - `Canvas`
      - `HUD`
        - `LivesText` (TextMeshPro - Text (UI))
  - `EventSystem`

### ✅ 시스템(상태 관리)
- `Main`
  - `Systems`
    - `GameState` (GameState.cs 붙은 오브젝트)

> `GameState`는 씬에서 1개만 존재하도록 싱글톤으로 구성합니다.

---

## 🧠 오늘 사용한 Unity 기술(키워드)
- **Singleton Pattern (MonoBehaviour Instance)**
- **SerializeField / Inspector 노출**
- **Trigger Collider / OnTriggerEnter**
- **TextMeshPro UI 업데이트**
- **RectTransform Anchor Preset / Pivot**
- **Canvas Scaler (Scale With Screen Size)**

---

## 1) GameState.cs (Life 상태 관리)

```csharp
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
        Debug.Log($"[GameState] Lives init: {lives}/{maxLives}");
    }

    public void LoseLife(int amount = 1)
    {
        if (IsGameOver) return;

        lives = Mathf.Max(0, lives - amount);
        Debug.Log($"[GameState] Lives: {lives}/{maxLives}");

        if (IsGameOver)
            Debug.Log("[GameState] GAME OVER");
    }

    [ContextMenu("Reset Lives")]
    public void ResetLives()
    {
        lives = maxLives;
        Debug.Log($"[GameState] Lives reset: {lives}/{maxLives}");
    }
}
```

---

## 2) Goal 도착 처리 (Life 감소 트리거)

Goal 오브젝트에 **Collider를 `Is Trigger`로 켜고**,  
Enemy가 들어오면 Life 감소를 호출합니다.

### 예시 흐름(구현 방식은 프로젝트에 맞게)
- Goal 오브젝트에 `GoalTrigger.cs` 같은 스크립트 부착
- `OnTriggerEnter(Collider other)`에서 Enemy 판별
- `GameState.Instance.LoseLife(1);`

```csharp
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Enemy 판별 방식은 본인 프로젝트 기준으로 맞추기
        if (!other.CompareTag("Enemy")) return;

        GameState.Instance?.LoseLife(1);

        // 필요하면 도착한 Enemy 제거/비활성화
        Destroy(other.gameObject);
    }
}
```

**✔️ 포인트:** “Enemy 판별 기준(Tag/Layer/Component)”은 프로젝트 기준으로 통일해두면 이후 확장 때 편합니다.

---

## 3) LivesText(TMP) — UI 표시 연결

### ✅ 역할
GameState의 Lives 값을 읽어서 TMP 텍스트에 표시

업데이트 방식은 2가지인데, 오늘은 **가장 단순한 방식(매 프레임 갱신)**으로 시작해도 충분합니다.

```csharp
using TMPro;
using UnityEngine;

public class LivesTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;

    private void Reset()
    {
        livesText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (GameState.Instance == null) return;
        livesText.text = $"Lives : {GameState.Instance.Lives}";
    }
}
```

> 나중에 최적화하고 싶으면 `LoseLife()`에서 이벤트를 쏘는 방식으로 바꾸면 됩니다.

---

## 4) HUD 위치 고정 (Anchor Preset)

### ✅ 오늘 겪은 문제
위치를 바꿨는데 생각보다 UI가 “제대로” 안 움직이는 느낌

### ✅ 해결
HUD 오브젝트의 **Anchor Preset을 코너 기준으로 고정**하니 원하는대로 동작

정리:
- HUD를 먼저 코너에 “붙이고”
- LivesText는 오프셋(Pos X/Y)로 미세 조절

---

## ✅ 체크리스트
- [x] GameState 씬에 1개 존재 (`Systems/GameState`)
- [x] Enemy가 Goal에 닿으면 Life 감소
- [x] TMP 텍스트가 Life 변화를 표시
- [x] HUD Anchor Preset 고정으로 위치 안정화
