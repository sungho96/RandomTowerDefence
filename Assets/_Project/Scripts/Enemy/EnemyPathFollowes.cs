using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFollowes : MonoBehaviour
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
        fixedY = transform.position.y; //오브젝트 바닥에서 시작 오브젝트 위치 보정 기능
    }

    /// <summary>
    /// 따라갈 경로를 생성받고 인덱스를 0으로 시작을 한다음 
    /// new path를 받아 enable기능을 통해 on/off 할수 있는 방어코드다.
    /// </summary>
    /// <param name="newpath"></param>
    public void Init(WayPointPath newpath)
    {
        path = newpath;//waypointPath.cs를 의미
        currentIndex = 0;

        if (lockY)
            fixedY = transform.position.y;

        enabled = (path != null && path.Points != null && path.Points.Count > 0);
    }
    private void Start()
    {   
        if(path == null)//Play시작할때 한번더 체크
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
        //인덱스가 0미만으로가는 버그 사전 방지
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

        //유니티에서 타겟위치로 전진하여 프레임단위로 균일하게 이동하게 하는 함수사용
        Vector3 nextPos = Vector3.MoveTowards(currentPos, targetPos,moveSpeed*Time.deltaTime);
        transform.position = nextPos;

        //회전이필요할경우 좀더 부드럽게 이용하기위해 Slerp를 활용함.
        if(rotateToMoveDirection)
        {
            Vector3 dir = (targetPos - currentPos);
            dir.y = 0f;
            //이동방향이 확실할 경우에만 이동하도록 수정
            if(dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }
        }

        //남은거리를 dist에 측정한다음 해당되면 AdvanceIndex() 호출
        float dist = Vector3.Distance(nextPos, targetPos);
        if (dist <= arriveDistance)
        {
            AdvanceIndex();
        }
    }

    /// <summary>
    /// 해당인덱스가 라스트 인덱스이고 loop 켜져있으면 다시 시작지점
    /// 아닐경우 목숨을 하나 줄이고 해당오브젝트 비활성화
    /// 마지막인데스가 아니라면 해당인덱스++
    /// </summary>
    private void AdvanceIndex()
    {
        int last = path.Points.Count - 1;

        if (currentIndex >= last)
        {
            if (loop)
                currentIndex = 0;
            else
            {
                if (GameState.Instance != null)
                    GameState.Instance.LossLife(1);
                    
                gameObject.SetActive(false);
            }
        }
        else
        {
            currentIndex++;
        }

    }

    /// <summary>
    /// 호출시 해당위치 텔레포트
    /// 해당함수는 해당 인덱스의 정확한 포지션을 확인하기위해 디버그용 만들었음.
    /// </summary>
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
