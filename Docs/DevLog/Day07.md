# 📅 Day 07 — 아군 타워 1차(탐지/공격 트리거) + 로그로 Hit 확인(HP 없이)

> 오늘은 “아군(타워)”의 최소 파이프라인을 열었습니다.  
> **적을 탐지하고(range), 일정 간격(hitInterval)으로 공격 시도(hit 로그)**가 찍히는지까지 확인했습니다.  
> Enemy HP/데미지는 아직 없으므로, Day07은 **로그로만 공격이 동작함을 증명**하는 단계입니다.

---

## 🎯 오늘의 목표
- 타워 오브젝트를 씬에 배치(고정 1개)
- 적 그룹(`Enemies`)을 기준으로 타겟 탐지
- 범위 내 가장 가까운 적을 선택
- 일정 간격으로 “Hit 로그” 출력(공격 트리거 확인)
- GameOver/TimeScale=0 상태에서 타워 동작이 중단되는 구조 유지

---

## ✅ 오늘 결론(구현 완료)
- `TowerShooter`를 통해
  - `Enemies` 아래의 적들을 순회하면서
  - 범위 내 적을 찾고
  - 일정 주기로 `Debug.Log`를 출력하는 공격 트리거를 성공적으로 확인했습니다.
- Enemy HP가 없어도 “아군 공격 루프”는 충분히 검증 가능하다는 걸 확인했습니다.

---

## 🧱 Hierarchy 구성 (정확한 경로)
- `Main/_SceneRoot/Gameplay/Towers`
  - `Tower_01`
    - `Visual` *(Fighters(pack) 휴먼 프리팹 배치)*
    - `Muzzle` *(공격 기준점 Transform)*
    - `Range` *(선택: 구조용/표시용)*

- `Main/_SceneRoot/Gameplay/Spawns/Enemies`
  - (런타임 생성 적들이 자식으로 들어오는 구조 유지)

---

## 🔧 Inspector 설정 체크
- `Tower_01 > TowerShooter (Script)`
  - `Enemiesparent` : `Main/_SceneRoot/Gameplay/Spawns/Enemies` 드래그 연결
  - `Range` : 테스트를 위해 충분히 큰 값으로 설정(예: 4~8)
  - `Hit Interval` : 예: 0.8
  - `Muzzle` : `Tower_01/Muzzle` 드래그 연결
  - `Rotate To Target` : 필요 시 On

---

## 🧠 오늘 핵심 디버깅 포인트(실제로 해결한 버그)
### 1) “가장 가까운 적 선택” 조건 연산자 방향 오류
- `bestDistSqr`의 초기값이 `float.MaxValue`인데
- 비교 연산자가 `>`로 되어 있으면 best가 갱신될 수 없어 타겟이 영원히 null이 됩니다.
- 따라서 비교는 반드시 `<`(더 가까울수록 갱신)로 처리해야 합니다.

### 2) Hit 로그 포맷(문자열 닫힘) 확인
- 로그 출력이 안 보일 땐, 문법적으로 문자열이 정상 닫혔는지(괄호/따옴표)부터 확인합니다.

---

## ✅ Play 테스트 성공 기준(오늘 완료 조건)
- 적이 타워 사거리 안으로 들어오면:
  - 콘솔에 `[TowerShooter] hit (...) -> EnemyName` 형태의 로그가 `hitInterval` 주기로 출력
- 적이 사거리 밖이면:
  - 로그가 출력되지 않음
- GameOver로 `Time.timeScale = 0` 상태가 되면:
  - 로그 출력이 즉시 중단됨

---

## ⚠️ 자주 나는 오류 3개 + 즉시 확인 방법
1) **타겟을 못 찾는 문제(항상 null)**
- 거리 비교 조건이 `<`인지 확인(가장 흔한 원인)
- `Enemiesparent.childCount`가 실제로 증가하는지(Hierarchy에서 런타임 확인)

2) **적이 있는데도 탐지 안 됨**
- 적 오브젝트(또는 자식)에 `EnemyPathFollowes` 컴포넌트가 존재하는지 확인
- `range`가 너무 작은지 확인(테스트용으로 6~8로 올려 검증)

3) **캐릭터(Visual) 때문에 적이 튕기거나 길 막힘**
- Visual 쪽 Collider가 과하게 붙어 있으면 임시로 비활성(테스트 목적)

---

## 🔜 다음 계획 (Day 08)
### 1) Enemy HP 시스템 도입
- `EnemyHealth` 추가(HP/TakeDamage/Death)
- 타워 공격이 “로그”가 아니라 “데미지 적용”으로 전환

### 2) 공격 표현(선택)
- Muzzle 기준으로 간단한 이펙트(라인/투사체) 또는 피격 이펙트 추가

### 3) 타겟 우선순위 확장(선택)
- 가장 가까운 적 외에 “진행도(Goal에 더 가까운 적) 우선” 등 정책 확장
