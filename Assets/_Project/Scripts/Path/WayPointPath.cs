using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPath : MonoBehaviour
{
    [Header("Auto-collected from Waypoints children")]
    [SerializeField] private List<Transform> points = new();

    public IReadOnlyList<Transform> Points => points;

    [SerializeField] private float gizmoYOffset = 0.05f;

    private void Awake()
    {
        if (points == null || points.Count == 0)
            CollectFromChildren();
    }
    private void OnValidate()
    {
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
