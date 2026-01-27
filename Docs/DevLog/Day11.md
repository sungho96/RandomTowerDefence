# 📅 Day 11 — Enemy HP 표시(TMP 3D 월드) 적용 + 가독성 확보

> 오늘은 “전투가 진행되고 있다”를 확실히 보여주기 위해  
> **Enemy 머리 위에 HP를 TextMeshPro(3D 월드 텍스트)** 로 표시했습니다.  
> 결과적으로 HP Bar까지 가지 않아도 **숫자만으로도 충분히 읽히고 자연스럽게** 동작하는 상태를 확보했습니다.

---

## 🎯 오늘의 목표
- Enemy에 **HP 표시(월드 3D 텍스트)** 추가
- 데미지/사망과 함께 **HP 텍스트가 즉시 갱신**
- 카메라 방향에 따라 뒤집히지 않도록(필요 시) **빌보드 처리**

---

## ✅ 오늘 구현 결과
- Enemy 스폰 시 머리 위에 `현재HP/최대HP` 형태로 HP가 보임
- 타워 타격 시 HP 감소 로그와 함께 텍스트도 즉시 변경됨
- Enemy가 죽어 Destroy 되면 텍스트도 함께 사라짐
- 현재 가독성이 충분해서 **HP Bar는 우선 패스 가능**한 상태

---

## 🧱 적용 Hierarchy(프리팹 내부)
- `EnemyPrefab`
  - `HPTextAnchor` (머리 위 위치 기준점)
    - `HPText` (TextMeshPro 3D)

> `HPTextAnchor`로 위치를 잡아두면  
> 적 모델/크기가 달라도 “머리 위” 위치만 쉽게 조절 가능합니다.

---

## 🔧 Inspector에서 한 일(핵심만)
- `HPTextAnchor`를 머리 위로 이동(캐릭터 키에 맞게 y값 조절)
- `HPText`는 **TextMeshPro(3D)** 컴포넌트인지 확인
  - (UI용 TextMeshProUGUI가 아니라 3D TextMeshPro)
- 가독성 확보:
  - Font Size / Transform Scale을 조절해 카메라 거리에서도 읽히게 맞춤

---

## ✅ 오늘 완료 기준(체크리스트)
- [ ] 적 머리 위에 HP 텍스트가 보인다
- [ ] 타워 타격 시 HP 텍스트가 즉시 감소한다
- [ ] 적 사망 시 텍스트도 함께 사라진다
- [ ] 플레이 중 HP가 “잘 읽히는 수준”으로 유지된다

---

## ⚠️ 자주 나는 오류 3개 + 즉시 확인 방법
1) **HP가 안 보임**
- `HPText`가 TextMeshProUGUI인지 확인 → 3D는 `TextMeshPro`여야 합니다
- Scale이 너무 작아서 안 보이는 경우가 많음 → 임시로 Scale 크게 해서 확인

2) **HP가 갱신이 안 됨**
- EnemyHealth에서 `TakeDamage()` 후 텍스트 갱신 호출 누락 가능
- 또는 hpText 참조가 Inspector에 연결 안 됐을 수 있음

3) **카메라 각도에 따라 뒤집혀 보임**
- 빌보드 스크립트(LateUpdate에서 카메라를 바라보게)를 붙이면 해결
- 당장 문제 없으면 생략해도 OK



