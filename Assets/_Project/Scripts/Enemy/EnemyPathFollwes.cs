using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFollwes : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private WayPointPath path;

    [Header("Move Setting")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float arriveDistance = 0.15f;

    [Header("Roatation Settings")]
    [SerializeField] private bool rotateToMoveDirection = true;
    [SerializeField] private float turnSpeed = 10f;

    [Header("End Behavior")]
    [SerializeField] private bool loop = false;

    [Header("Height Handling")]
    [SerializeField] private bool lockY = true;
    [SerializeField] private float fixedY = 0f;

    private int currentIndex = 0;

    private void Reset()
    {
        fixedY = transform.position.y;
    }

    public void Init(WayPointPath newpath)
    {
        path = newpath;
        currentIndex = 0;

        if (lockY)
            fixedY = transform.position.y;

        enabled = (path != null && path.Points != null && path.Points.Count > 0);
    }
    private void Start()
    {
        if(path == null)
        {
            Debug.LogError("[EnemyPathFollwer] path refernce is missing.", this);
            enabled = false;
            return;
        }
        if (path.Points == null || path.Points.Count == 0)
        {
            enabled = false;
            return;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, path.Points.Count -1);
    }

    private void Update()
    {
        var points = path.Points;
        if (points == null || points.Count == 0) return;

        Transform target = points[currentIndex];
        if (target == null)
        {
            AdvanceIndex();
            return;
        }
        Vector3 targetPos = target.position;
        Vector3 currentPos = transform.position;

        if (lockY)
        {
            currentPos.y = fixedY;
            targetPos.y = fixedY;
        }

        Vector3 nextPos = Vector3.MoveTowards(currentPos, targetPos,moveSpeed*Time.deltaTime);
        transform.position = nextPos;

        if(rotateToMoveDirection)
        {
            Vector3 dir = (targetPos - currentPos);
            dir.y = 0f;

            if(dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }
        }

        float dist = Vector3.Distance(nextPos, targetPos);
        if (dist <= arriveDistance)
        {
            AdvanceIndex();
        }
    }

    private void AdvanceIndex()
    {
        int last = path.Points.Count - 1;   

        if (currentIndex >= last)
        {
            if (loop)
                currentIndex = 0;
            else
                enabled = false;
        }
        else
        {
            currentIndex++;
        }
    }
    [ContextMenu("Snap to Current WayPoint")]
    private void SnapToCurrentWaypoint()
    {
        if (path == null || path.Points == null || path.Points.Count == 0) return;
        Transform t = path.Points[Mathf.Clamp(currentIndex, 0, path.Points.Count - 1)];
        if (t == null) return;

        Vector3 p = t.position;
        if (lockY) p.y = fixedY;
        transform.position = p;
    }
}
