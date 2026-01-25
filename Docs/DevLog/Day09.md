# 📅 Day 09 — Tower FSM(Idle/Attack) + Animator Controller 전환 + 공격 템포 맞추기

> 오늘은 타워 전투를 “게임처럼” 보이게 만드는 핵심인  
> **대기/공격 FSM**과 **애니메이션 컨트롤러 전환**을 붙였습니다.  
> 또한 공격 모션이 너무 느리게 느껴지는 문제를 **애니메이션 속도/공격 간격**으로 맞추는 방향까지 정리했습니다.  
> (HP UI Bar는 작업량이 커서 다음 세션으로 미루기로 결정)

---

## 🎯 오늘의 목표
- 타워가 적을 감지하면 **Idle → Attack 상태 전환**
- 상태에 따라 **Animator Controller(대기/공격) 전환**
- 공격 모션 템포가 느린 문제를 해결하기 위한 **속도/간격 정합** 방향 확정
- (보류) Enemy HP UI Bar는 다음으로 미루기

---

## ✅ 오늘 구현 결과
### 1) FSM(Idle/Attack) 도입
- 타워가 타겟을 잡으면 `Attack` 상태로 전환
- 타겟이 없으면 `Idle` 상태로 복귀
- 상태 전환은 중복 실행되지 않도록 가드(같은 상태면 무시)

### 2) Animator Controller 2개 전환
- `idleController` / `attackController` 준비
- 상태가 바뀌는 순간 `Animator.runtimeAnimatorController` 교체
- 공격 상태 진입 시 공격 모션이 재생되도록 구성

### 3) 공격 동작은 기존 방식 유지
- 감지(타겟 탐색) → 바라보기 → hitInterval마다 데미지 적용
- GameOver/TimeScale=0 방어 코드 유지

---

## 🧱 적용 위치(정확한 구조)
- `Main/_SceneRoot/Gameplay/Towers/Tower_01`
  - `TowerShooter` (FSM + 공격 로직)
- `Main/_SceneRoot/Gameplay/Towers/Tower_01/Visual`
  - `Animator` (컨트롤러 전환 대상)

---

## 🔧 Inspector 연결(오늘 핵심)
### TowerShooter 컴포넌트
- `animator` : `Tower_01/Visual`의 Animator 연결
- `idleController` : 대기 Animator Controller 에셋 연결
- `attackController` : 공격 Animator Controller 에셋 연결
- `range / hitInterval / damage / muzzle` 기존대로 유지

---

## 🧠 문제 해결: 공격 모션이 너무 느릴 때(정합 전략)
### 결론: "데미지 타이밍(hitInterval)"을 기준으로 맞추는 게 관리가 쉽다
- 게임 밸런스 수치는 `hitInterval`이 기준이 되기 쉬움
- 따라서 애니메이션 템포를 `hitInterval`에 맞추는 방식이 안전

### 해결 방법(오늘 방향 확정)
1) **attackController 내 공격 State Speed 올리기**
   - 1.5 → 2.0 → 2.5 식으로 단계적으로 테스트
2) 필요하면 `hitInterval`도 모션 템포에 맞춰 조정
   - 모션이 0.6초 느낌이면 hitInterval도 0.6 근처로

---

## ✅ 오늘 완료 기준(체크리스트)
- [ ] 적이 사거리 안에 들어오면 Idle → Attack 상태로 전환된다
- [ ] Attack 상태에서 공격 컨트롤러로 바뀌며 공격 모션이 재생된다
- [ ] 타겟이 없으면 Idle로 돌아가며 대기 컨트롤러로 복귀한다
- [ ] 공격 모션이 느린 문제를 Speed/hitInterval로 맞추는 방향을 확정했다
- [ ] Enemy HP UI Bar는 다음 세션으로 미루기로 결정했다

---

## ⚠️ 자주 나는 오류 3개 + 즉시 확인 방법
1) **공격 모션이 안 바뀜**
- `TowerShooter.animator`가 진짜 `Visual`의 Animator를 참조하는지 확인
- Play 모드에서 `Visual` 선택 후 Controller가 바뀌는지 확인

2) **컨트롤러는 바뀌는데도 모션이 Idle처럼 보임**
- `attackController`의 기본 상태(Default State)가 공격이 맞는지 확인
- 공격 클립이 실제로 연결되어 재생되는지(Animator 창에서 하이라이트) 확인

3) **공격 모션은 느린데 데미지는 빨리/늦게 들어가 어색함**
- 데미지는 `hitInterval` 기준으로 들어감
- 공격 모션 템포는 State Speed로 맞추고, 필요하면 hitInterval도 같이 조정

---

## 🔜 다음 계획
### Day 10 후보(택 1)
1) **이펙트 1개(발사 or 히트)만** “Hit 타이밍”에 붙이기  
   - 시각 피드백 강화(더 게임 같아짐)
2) **Enemy HP UI Bar(월드 스페이스)** 작업 시작  
   - UI 작업량이 크므로 별도 세션으로 잡는 게 안전

> HP UI Bar는 UI/카메라/정렬까지 얽혀서,  
> 오늘처럼 전투 로직 안정화가 끝난 다음 세션에 진행하는 게 효율적이라고 판단.

