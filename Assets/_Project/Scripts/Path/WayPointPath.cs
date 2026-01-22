using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPath : MonoBehaviour
{
    [Header("Auto-collected from Waypoints children")]
    [SerializeField] private List<Transform> points = new();

    public IReadOnlyList<Transform> Points => points;

    [SerializeField] private float gizmoYOffset = 0.05f; //z-fighting 발생하여 pos.y 값보정

    private void Awake()
    {
        if (points == null || points.Count == 0) //Waypoint가 등록이 안되어있을때 자동 수집기능 
            CollectFromChildren();
    }
    private void OnValidate() //값이 변경시 자동호출하여 방어코드 구현
    {
        CollectFromChildren();
    }
    /// <summary>
    /// 시작시 리스트 초기화 인덱스 순서에맞게 waypoint를 List에 추가
    /// </summary>
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
    /// <summary>
    /// 에디터 모드일때 Waypoint의 위치와 추후에 움직이는 노선을 알기 위해 사용함.
    /// </summary>
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
