# 📅 Day 08 — Enemy HP / TakeDamage / Death 연출(기울기+축소+소멸) + 죽는 즉시 이동 중단

> 오늘은 타워가 “Hit 로그”만 찍던 단계에서 한 단계 올라가,  
> **Enemy HP 시스템을 붙이고(TakeDamage), HP 0이면 죽음 처리(연출 후 소멸)**까지 완성했습니다.  
> 죽는 순간에는 **즉시 이동을 멈추고**, 쓰러지면서 **기울어지고 작아지며 사라지는** 연출을 적용했습니다.

---

## 🎯 오늘의 목표
- Enemy에 HP 도입 (`EnemyHealth`)
- 타워 공격이 “로그”가 아니라 “데미지 적용”으로 동작
- HP 0이 되는 순간:
  - 죽음 처리 1회만 발생(가드)
  - 이동 즉시 중단
  - **기울기 + 축소 + 소멸** 연출 후 제거

---

## ✅ 오늘 구현 결과
### ✅ EnemyHealth(HP 시스템) 추가
- `maxHp / currentHp`로 HP 관리
- `TakeDamage(amount)`로 데미지 처리
- HP 로그 출력: `[EnemyHealth] ... HP : current/max`

### ✅ Death 연출(더 게임답게)
- HP 0이면 `Die()` 실행
- `DieRoutine()`로:
  - 기울어짐(tiltAngle)
  - 축소(ShrinkScale)
  - 일정 시간(deathDuration) 후 Destroy

### ✅ 죽는 즉시 이동 중단
- `EnemyPathFollowes`를 죽는 순간 `enabled=false`로 꺼서  
  쓰러지는 동안 이동하지 않게 처리

---

## 🧱 적용 대상(정확한 위치)
- **Enemy 프리팹(원본)**에 `EnemyHealth` 컴포넌트 추가(Apply)
- 타워는 기존 `TowerShooter`가 `EnemyHealth.TakeDamage()`를 호출하는 구조 유지

---

## 🔧 Inspector 파라미터(테스트 기준값)
### EnemyHealth (Script)
- `maxHp`: 10 (테스트)
- `deathDuration`: 0.45
- `tiltAngle`: 85
- `ShrinkScale`: 0.5 *(더 강하게 줄이고 싶으면 0.1~0.2 권장)*
- `disableMoveMentOnDeath`: ON

---

## 🧠 오늘 핵심 포인트(버그/가드)
### 1) 죽음 처리 중복 방지 가드
- `isDying` 플래그로 `Die()`가 여러 번 실행되지 않게 막음
- `TakeDamage()`에서도 `isDying` 체크로 “죽는 중 추가 피격” 방지

### 2) 죽는 순간 이동 즉시 중단
- `EnemyPathFollowes`를 꺼서 이동이 바로 멈추도록 처리
- 죽음 연출이 “자리에서” 진행되게 만듦

### 3) 충돌로 튕기는 문제 예방(선택)
- `DieRoutine()`에서 Collider를 꺼서 쓰러지는 동안 충돌 영향을 줄임

---

## ✅ Play 테스트 성공 기준(오늘 완료 조건)
- 타워 사거리 안에 적이 들어오면:
  - Enemy HP 로그가 hitInterval마다 감소
- HP가 0이 되는 순간:
  1) `[EnemyHealth] ... Dead` 로그 1회 출력
  2) 적 이동 즉시 멈춤
  3) 적이 옆으로 기울어짐
  4) 크기가 줄어들며
  5) 일정 시간 후 사라짐(Destroy)

---

## ⚠️ 자주 나는 오류 3개 + 즉시 확인 방법
1) **HP가 안 줄어듦**
- Enemy 프리팹 “원본”에 `EnemyHealth`가 붙었는지 확인(Apply 했는지)
- 타워가 `EnemyHealth.TakeDamage()`를 호출하고 있는지 확인

2) **죽음 연출이 안 나옴 / 즉시 사라짐**
- `Die()` 가드 조건이 정상인지 확인 (`isDying` 중복 방지)
- `deathDuration`이 너무 짧게 설정된 건 아닌지 확인

3) **죽는 동안 계속 맞는 로그가 난사됨**
- `TakeDamage()` 시작에 `if (isDying) return;`이 있는지 확인
- TowerShooter hitInterval이 너무 짧은지 확인

---

## 🔜 다음 계획 (Day 09)
- 타워 공격 표현 추가(간단한 이펙트/라인/투사체 중 택1)
- 타겟 우선순위 정책 확장(가까운 적 vs 진행도 우선)
- (선택) Enemy HP UI(월드 스페이스 바) 또는 피격 피드백 추가

