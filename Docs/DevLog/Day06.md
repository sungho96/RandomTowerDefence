# 📅 Day 06 — GameOver Popup(Confirm → Retry/Exit) 구현 + OnGameOver 이벤트 유지 + 버튼 OnClick은 코드 연결

> 오늘은 GameOver를 “진짜 게임처럼” 완성하는 핵심인 **팝업 흐름(Confirm → Options)** 을 구현했습니다.  
> GameOver 발생은 `GameState`가 **OnGameOver 이벤트를 울리고**, UI가 그 알림을 받아 **팝업을 표시**하는 구조로 유지했습니다.  
> 버튼 클릭은 Inspector가 아니라 **코드에서 OnClick을 직접 연결**하는 방식으로 고정했습니다.

---

## 🎯 오늘의 목표
- Lives가 0이 되는 순간 GameOver 팝업 표시(이벤트 기반)
- 팝업 UI를 2단 구조로 구성
  - **Confirm 먼저 표시**
  - Confirm 클릭 후 **Retry/Exit 표시**
- 버튼은 MUIP 오브젝트 사용 + **Unity Button 컴포넌트만 추가**
- 버튼 클릭은 **코드에서 연결**(Inspector OnClick 비움)

---

## ✅ 오늘 결론(설계/구현 결정)
### ✅ GameOver UI 정책
- GameOver 발생 시:
  1) `GameState`에서 `OnGameOver` 이벤트 발생
  2) `GameOverUI`가 이벤트를 받아 팝업 활성화
  3) 팝업은 **Confirm 패널부터** 시작
  4) Confirm 클릭 후 **Options(Retry/Exit)** 표시

### ✅ 버튼 정책
- 버튼 외형은 MUIP
- 클릭 입력은 Unity `Button` 컴포넌트로 받음
- OnClick 연결은 **코드에서만 처리**

---

## 🧱 Hierarchy 구성 (정확한 경로)
- `Main/_SceneRoot/UI/Canvas/UIController`
  - `Popup_Gameover` *(비활성 시작)*
    - `GameOver_Panel`
      - `Panel_Confirm` *(Active ON)*
        - `Confirm Button` *(MUIP 오브젝트 + Unity Button 컴포넌트)*
      - `Panel_Options` *(Active OFF)*
        - `Retry Button` *(MUIP 오브젝트 + Unity Button 컴포넌트)*
        - `Exit Button` *(MUIP 오브젝트 + Unity Button 컴포넌트)*

---

## 🔧 Inspector 설정(오늘 체크 포인트)
### 1) Popup/패널 Active 상태
- `Popup_Gameover` : **비활성 시작 유지**
- `Panel_Confirm` : **Active = ON**
- `Panel_Options` : **Active = OFF**

### 2) 버튼 컴포넌트 규칙
- `Confirm Button`, `Retry Button`, `Exit Button` 오브젝트 각각:
  - `Add Component → Button`(Unity Button) 추가
  - Unity Button의 **OnClick 리스트는 비워둠**

---

## 🧠 오늘 배운 핵심 개념 (짧게 정리)

### 1) `OnGameOver` 이벤트를 “계속 쓰기로” 결정한 이유
- GameOver는 “한 번 발생하면 여러 시스템(UI/사운드/스폰 중단 등)이 반응”하는 이벤트라서  
  `GameState`가 **알림만 보내고**, UI가 **구독해서 반응**하는 구조가 확장에 유리합니다.

### 2) `OnEnable/OnDisable`에서 `+= / -=`를 쓰는 이유(안전장치)
- `OnEnable` : 구독 등록(이제부터 GameOver 알림 받기)
- `OnDisable` : 구독 해제(중복 등록/메모리 꼬임 방지)
- 이 짝이 깨지면 “팝업이 두 번 뜨는” 문제가 생기기 쉬움

---

## ✅ Play 테스트 성공 기준(오늘 완료 조건)
- Lives가 0이 되는 순간:
  - 콘솔에 `[GameState] Game over` 로그가 **1번만** 출력
  - `Popup_Gameover`가 켜짐
  - 처음에는 **Confirm만 보임**(Options는 숨김)
- Confirm 클릭:
  - Confirm 숨김 → Retry/Exit 표시
- Retry 클릭:
  - (GameOver에서 timeScale=0을 사용했다면) **timeScale 복구 후 씬 재시작**
- Exit 클릭:
  - 에디터에서는 로그 / 빌드에서는 종료 동작

---

## ⚠️ 자주 나는 오류 3개 + 즉시 확인 방법
1) **버튼 클릭이 안 됨**
- `EventSystem` 존재 확인  
- 버튼 오브젝트에 Unity `Button` 컴포넌트가 붙었는지 확인

2) **Confirm 눌러도 Retry/Exit가 안 뜸**
- `Panel_Confirm / Panel_Options` 연결(드래그) 누락 확인  
- 기본 Active 상태가 Confirm ON / Options OFF인지 확인

3) **Retry 후에도 멈춘 상태로 시작**
- Retry 처리에서 `Time.timeScale = 1f` 복구가 있는지 확인

---

## 🔜 다음 계획 (Day 07) — 아군(타워) 시작
### ✅ Day07은 Enemy HP 없어도 진행 가능
- 목표는 “아군 기능 파이프라인”을 먼저 여는 것:
  - 타워 배치(일단은 고정 1개 배치)
  - 적 탐지(범위 내)
  - 공격 트리거(로그 또는 1타 제거로 확인)

### Day08에서 HP 정식화
- `EnemyHealth`(HP/TakeDamage/사망 처리) 추가
- 타워 데미지 수치 반영, 사망 정책 정리

