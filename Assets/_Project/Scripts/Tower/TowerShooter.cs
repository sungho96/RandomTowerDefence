using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerShooter : MonoBehaviour
{
    [Header("Target Source")]
    // Main/_SceneRoot/Gameplay/Enemies
    [SerializeField] private Transform enemiesparent;

    [Header("Attack Seetings")]
    [SerializeField] private float range = 2.0f; //공격범위
    [SerializeField] private float hitInterval = 0.8f;

    [Header("Optional")]
    [SerializeField] private Transform muzzle; 
    [SerializeField] private bool rotateToTarget = true;

    private float nextHitTime = 0f;

    private void Update()
    {
        // GameOver/일시정지(timeScale=0)일 때 자동 방어 코드
        if (GameState.Instance != null && GameState.Instance.IsGameOver) return;
        if (Time.timeScale == 0f) return;
        if (enemiesparent == null) return;

        Transform target = FindNearestEnemyInRange();//근처 적 탐색
        if (target == null) return;

        // y축고정한 상태로 타켓을 바라보도록 회전
        if (rotateToTarget)
        {
            Vector3 lookpos = target.position;
            lookpos.y = transform.position.y;
            transform.LookAt(lookpos);
        }
        //일정간격마다 히트처리
        if (Time.time >= nextHitTime)
        {
            nextHitTime = Time.time + hitInterval;

            string from = (muzzle != null) ? muzzle.name : gameObject.name;
            Debug.Log($"[TowerShooter] hit ({from} -> {target.name})");
        }

    }
    /// <summary>
    /// 적부모(enemiesparent)의 자식들중 
    /// 활성상태이며
    /// EnemyPathFollowes 컴포넌트를 가진 적이며
    /// 사거리안에 있는 대사중 
    /// 가장가까운 적 공격
    /// 공격 기준점은 muzzle 있으면 muzzle 아니면 tower
    /// </summary>
    /// <returns></returns>
    private Transform FindNearestEnemyInRange()
    {
        float rangeSqr = range * range; //sqrt(cpu)연상량 때문에 일부러사용
        Transform best = null;
        float bestDisSqr = float.MaxValue;

        Vector3 origin = (muzzle != null) ? muzzle.position : transform.position;

        for (int i=0; i < enemiesparent.childCount; i++)
        {
            Transform t = enemiesparent.GetChild(i);
            if (!t.gameObject.activeInHierarchy) continue;//꺼진 오브젝트는 제외 

            // 적이 EnemyPathFollowes를 가진 오브젝트(또는 자식)라도 잡히게 최소 보정
            EnemyPathFollowes follower = t.GetComponentInChildren<EnemyPathFollowes>();//이게있어야하는가? 이미 안에있으면있을텐데
            if (follower == null) continue;

            Transform enemyRoot = follower.transform;

            //orgin으로 부터 거리^2
            float dSqr = (enemyRoot.position - origin).sqrMagnitude;

            //사거리 안에있고 더가까우면 갱신
            if (dSqr <= rangeSqr && dSqr < bestDisSqr)
            {
                bestDisSqr = dSqr;
                best = enemyRoot;
            }
        }
        return best;
    }
    /// <summary>
    /// 에디터모드에서 공격범위 육안으로 확인
    /// </summary>
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = (muzzle != null) ? muzzle.position : transform.position;
        Gizmos.DrawWireSphere(origin, range);
    }
#endif
}
