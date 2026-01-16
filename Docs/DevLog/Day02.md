# 📅 Day 2 — L자 Enemy Path + Waypoints + Gizmo 시각화

## 🎯 오늘의 목표
- Enemy가 이동할 **L자 경로(폭 1)**를 만들고
- 모서리마다 **WayPoint(WP_00~)** 배치
- WayPointPath 스크립트로 **자동 수집 + Gizmo로 경로 시각화**
- Z-fighting 방지를 위해 Gizmo 라인을 **바닥보다 살짝 띄워서** 표시

---

## 🧠 오늘 작업에서 배운/사용한 Unity 기술(키워드)
- **Transform 계층(Parent-Child)**
  - `WayPoints` 오브젝트 아래에 WP들을 자식으로 정리 → 스크립트에서 자동 수집 가능
- **[SerializeField] + Inspector 리스트 디버깅**
  - 런타임 전/후에 points 리스트가 제대로 구성되는지 Inspector로 확인
- **OnValidate()**
  - 에디터에서 오브젝트 변경 시 자동 갱신 (WayPoint 수집 자동화)
- **[ContextMenu]**
  - Inspector에서 우클릭 메뉴로 Collect 실행 가능 (수동 갱신 보조)
- **OnDrawGizmos()**
  - 씬/게임 화면에서 경로를 선/점으로 표시해 디버깅 효율 상승
- **Z-Fighting 대응**
  - Gizmo 좌표에 `Vector3.up * offset`을 더해 바닥과 겹침 방지

---

## ✅ 오늘 구현 결과
- WayPoint들을 `WayPoints` 부모 아래에 `WP_00 ~ WP_05` 형태로 정리
- WayPointPath가 자동으로 자식들을 수집해서 points 리스트 구성
- Gizmo로 경로가 노란 선/점으로 표시되어, Enemy 이동 구현 전에도 경로 검증 가능

---

## 🧩 WayPointPath.cs (오늘 작성한 전체 코드)
```csharp
using System.Collections.Generic;
using UnityEngine;

public class WayPointPath : MonoBehaviour
{
    [Header("Auto-collected from Waypoints children")]
    [SerializeField] private List<Transform> points = new();

    [Header("Gizmo")]
    [SerializeField] private float gizmoYOffset = 0.05f; // z-fighting 방지용 (5cm)

    public IReadOnlyList<Transform> Points => points;

    private void Awake()
    {
        // 플레이 시작 시 points가 비어있으면 자동 수집
        if (points == null || points.Count == 0)
            CollectFromChildren();
    }

    private void OnValidate()
    {
        // 에디터에서 자식 구조 바뀌면 자동으로 다시 수집
        CollectFromChildren();
    }

    [ContextMenu("Collect From Children")]
    public void CollectFromChildren()
    {
        points.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null)
                points.Add(child);
        }
    }

    private void OnDrawGizmos()
    {
        if (points == null || points.Count < 2) return;

        Gizmos.color = Color.yellow;

        Vector3 offset = Vector3.up * gizmoYOffset;

        for (int i = 0; i < points.Count; i++)
        {
            Transform p = points[i];
            if (p == null) continue;

            Vector3 a = p.position + offset;
            Gizmos.DrawSphere(a, 0.12f);

            if (i < points.Count - 1 && points[i + 1] != null)
            {
                Vector3 b = points[i + 1].position + offset;
                Gizmos.DrawLine(a, b);
            }
        }
    }
}
