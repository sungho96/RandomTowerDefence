# 📅 Day 10 — Hit VFX(칼 타격) 연결 + 위치(Y 오프셋) + 방향(회전) 정렬

> 오늘은 타워 공격이 “게임처럼 보이게” 만드는 핵심인 **Hit 이펙트(VFX)** 를 붙였습니다.  
> 단순히 나오게만 한 게 아니라, 실제 플레이에서 어색하지 않도록  
> **위치(너무 낮게 뜨는 문제)** 와 **방향(파티클 방향 정렬)** 까지 안정화했습니다.

---

## 🎯 오늘의 목표
- 타워 타격 시 **Hit VFX 프리팹**이 나오도록 연결
- 이펙트가 너무 낮게 뜨는 문제 해결(**Y 오프셋**)
- 파티클 방향이 안 맞는 문제 해결(**회전 정렬**)
- 프리팹에서 회전이 안 먹는 케이스 대응(Instantiate 회전 기반)

---

## 🧱 적용 구조(정확한 위치)
- `Main/_SceneRoot/Gameplay/Towers/Tower_01`
  - `TowerShooter` : Hit 타이밍(데미지 적용 순간)에 VFX 생성
  - `Muzzle` : 기준점(시각적으로 공격 방향 확인용, 칼이라도 유지)

---

## ✅ 오늘 구현 결과

### 1) Hit VFX 프리팹 연결(1개만 먼저)
- TowerShooter에 Hit VFX 프리팹 참조 필드 추가
- 타격이 실제로 들어가는 순간(HP 감소 로직 직후)에 VFX Instantiate

### 2) 너무 낮게 뜨는 문제 해결
- 적의 Pivot이 발바닥/루트에 있어서 VFX가 바닥에 붙는 문제가 발생
- 해결: `hitFxYOffset`로 `target.position + Vector3.up * hitFxYOffset` 방식 적용

### 3) 파티클 방향 정렬(프리팹 회전이 안 먹히는 케이스 대응)
- 프리팹에서 회전이 기대처럼 적용되지 않는 경우가 발생
- 해결: Instantiate 시점에 **Quaternion을 직접 계산**해서 회전 적용
  - 기본 방향: 타워 → 타겟 방향
  - 축이 90/180 틀어진 경우를 위해 `hitFxRotationOffset`로 보정 가능

---

## 🔧 Inspector에서 한 일(연결 포인트)
- `Main/_SceneRoot/Gameplay/Towers/Tower_01` 선택
  - `TowerShooter`
    - `Hit Fx Prefab` : 준비된 Hit VFX 프리팹 드래그
    - `Hit Fx Life Time` : 1.0~2.0(기본 1.5로 시작)
    - `Hit Fx Y Offset` : 1.0 근처에서 조절(몸통 높이로 맞춤)
    - `Hit Fx Rotation Offset` : 필요 시 (0, 90, 0) / (0, -90, 0) / (0, 180, 0) 등으로 미세 조정

---

## ✅ 오늘 완료 기준(체크리스트)
- [ ] 타워가 데미지를 줄 때마다 Hit VFX가 1회 재생된다
- [ ] VFX가 바닥이 아니라 몸통/피격 지점 근처에서 보인다(Y 오프셋 조절 완료)
- [ ] VFX 방향이 자연스럽게 정렬된다(Instantiate 회전 적용)
- [ ] 축이 틀어진 케이스는 Rotation Offset 값으로 조정 가능하다

---

## ⚠️ 자주 나는 오류 3개 + 즉시 확인 방법
1) **VFX가 안 나옴**
- `TowerShooter`의 `Hit Fx Prefab` 슬롯이 비어있지 않은지 확인
- 타격 로그/HP 감소 로그가 뜨는지 확인(타격 자체가 발생하는지)

2) **VFX 위치가 계속 어색함(너무 낮거나 너무 높음)**
- `hitFxYOffset` 값을 0.6~1.5 범위에서 조정
- 적 캐릭터 Pivot이 발바닥이면 오프셋이 필수인 구조임

3) **VFX 방향이 이상함 / 프리팹 회전이 안 먹는 느낌**
- Instantiate 회전(타워→타겟 LookRotation) 기반으로 처리하는 게 안정적
- 90/180도 틀어지면 `hitFxRotationOffset` 값으로 보정

---

## 🔜 다음 계획(Day 11 후보)
### 후보 A) Enemy HP UI Bar(월드 스페이스)
- EnemyHealth와 UI Fill을 연결해서 HP 감소를 “눈으로” 확인 가능하게 만들기
- 월드 캔버스/카메라 방향(빌보드)/스케일 조정 포함

### 후보 B) 타격 타이밍 더 정교화(애니메이션에 맞춰 히트 타이밍)
- 지금은 hitInterval 기준으로 안정화가 우선이라 잘 작동함
- 더 욕심내면 “타격 모션 중간 프레임”에 VFX+데미지 타이밍을 맞추는 방향으로 확장 가능

> 오늘은 VFX의 위치/방향까지 안정화했기 때문에  
> 다음은 “정보 UI(HP Bar)” 또는 “타이밍 정교화” 중 하나로 가는 게 효율적입니다.

