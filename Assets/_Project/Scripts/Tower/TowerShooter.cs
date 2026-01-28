using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Video;

// FSM
public class TowerShooter : MonoBehaviour
{
    [Header("Target Source")]
    // Main/_SceneRoot/Gameplay/Enemies
    [SerializeField] private Transform enemiesparent;

    [Header("Attack Seetings")]
    [SerializeField] private float range = 2.0f; // 공격 범위
    [SerializeField] private float hitInterval = 0.8f;
    [SerializeField] private int damage = 1;

    [Header("Optional")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private bool rotateToTarget = true;

    // FSM
    [Header("FSM / Anim Controller")]
    [SerializeField] private Animator animator;
    [SerializeField] private RuntimeAnimatorController idleController; //대기 
    [SerializeField] private RuntimeAnimatorController attackController; //공격

    [Header("VFX")]
    [SerializeField] private GameObject hitFxPrefab; 
    [SerializeField] private float hitFxLifeTime = 1.5f; //프리팹 라이프타임
    [SerializeField] private float hitFxYOffset = 1.0f;

    [Header("SFX")]
    [SerializeField] private AudioSource hitAudiSource; //오디오소스
    [SerializeField] private AudioClip hitSfx; //오디오클립
    [SerializeField, Range(0f, 1f)] private float hitSfxVolume = 0.8f;

    private enum TowerState { Idle, Attack }
    private TowerState state = TowerState.Idle;

    private float nextHitTime = 0f;

    private void Update()
    {
        // GameOver / 일시정지(timeScale=0) 상태일 때는 동작하지 않도록 처리
        if (GameState.Instance != null && GameState.Instance.IsGameOver) return;
        if (Time.timeScale == 0f) return;
        if (enemiesparent == null) return;

        Transform target = FindNearestEnemyInRange(); // 사거리 내 가장 가까운 적 탐색

        // 타겟이 없으면 Idle 상태로 전환 후 종료
        if (target == null) { SetState(TowerState.Idle); return; }

        // 타겟이 있으면 Attack 상태로 전환
        SetState(TowerState.Attack);

        // Y축 회전은 고정(수평 회전만)하여 타겟을 바라보게 함
        if (rotateToTarget)
        {
            Vector3 lookpos = target.position;
            lookpos.y = transform.position.y;
            transform.LookAt(lookpos);
        }

        // 공격 쿨타임 체크
        if (Time.time >= nextHitTime)
        {
            nextHitTime = Time.time + hitInterval;

            string from = (muzzle != null) ? muzzle.name : gameObject.name;

            EnemyHealth hp = target.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                // 데미지 적용
                hp.TakeDamage(damage);

                // hit VFX 생성
                if (hitFxPrefab != null)
                {
                    Vector3 fxPos = target.position + Vector3.up * hitFxYOffset; //너무아래서 생겨서 보정

                    Quaternion fxRot = Quaternion.identity;

                    Vector3 dir = (target.position - transform.position);// 방향설정
                    dir.y = -30f;

                    if (dir.sqrMagnitude > 0.001f)
                        fxRot = Quaternion.LookRotation(dir.normalized);//그쪽을 바라보면서 공격

                    GameObject fx = Instantiate(hitFxPrefab, fxPos, fxRot);
                    Destroy(fx, hitFxLifeTime);
                }

                // hit SFX 재생
                if (hitSfx != null)
                {
                    if (hitAudiSource != null)
                    {
                        hitAudiSource.PlayOneShot(hitSfx, hitSfxVolume); //한번실행
                    }
                    else
                    {
                        Vector3 sfxPos = (muzzle != null) ? muzzle.position : transform.position;
                        AudioSource.PlayClipAtPoint(hitSfx, sfxPos, hitSfxVolume); //3d에서 자동실행(파괴까지)
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[TowerShooter] EnemyHealth not found on{target.name}");
            }

            Debug.Log($"[TowerShooter] hit ({from} -> {target.name})");
        }
    }

    /// <summary>
    /// enemiesParent(적 컨테이너) 하위에서,
    /// 현재 사거리(range) 안에 있는 적들 중 가장 가까운 적을 반환
    /// - 비활성 오브젝트는 제외
    /// - EnemyPathFollowes가 붙은 오브젝트만 “유효한 적”으로 판단
    /// - 거리 계산은 sqrMagnitude를 사용해 sqrt 연산 비용을 줄임
    /// - 기준점(origin)은 muzzle이 있으면 muzzle, 없으면 tower 위치를 사용
    /// </summary>
    private Transform FindNearestEnemyInRange()
    {
        float rangeSqr = range * range; // sqrt 비용을 피하기 위해 제곱 거리로 비교
        Transform best = null;
        float bestDisSqr = float.MaxValue;

        Vector3 origin = (muzzle != null) ? muzzle.position : transform.position;

        for (int i = 0; i < enemiesparent.childCount; i++)
        {
            Transform t = enemiesparent.GetChild(i);
            if (!t.gameObject.activeInHierarchy) continue; // 비활성 오브젝트 제외

            // EnemyPathFollowes가 없는 오브젝트는 적이 아니라고 판단(예: 이펙트/기타 오브젝트)
            EnemyPathFollowes follower = t.GetComponentInChildren<EnemyPathFollowes>(); // 자식 포함 탐색
            if (follower == null) continue;

            Transform enemyRoot = follower.transform;

            // origin 기준 제곱 거리
            float dSqr = (enemyRoot.position - origin).sqrMagnitude;

            // 사거리 내 + 더 가까운 적이면 갱신
            if (dSqr <= rangeSqr && dSqr < bestDisSqr)
            {
                bestDisSqr = dSqr;
                best = enemyRoot;
            }
        }
        return best;
    }

    /// <summary>
    /// FSM 상태 전환 처리
    /// - 같은 상태면 무시
    /// - Animator가 있으면 상태에 맞는 RuntimeAnimatorController(idle/attack)로 교체
    /// - 이미 같은 컨트롤러면 불필요한 교체 x
    /// </summary>
    private void SetState(TowerState next)
    {
        if (state == next) return;
        state = next;

        if (animator == null) return;

        RuntimeAnimatorController targetController =
            (state == TowerState.Attack) ? attackController : idleController;

        // 이미 같은 컨트롤러면 교체하지 않음(불필요한 변경 방지)
        if (targetController != null && animator.runtimeAnimatorController != targetController)
            animator.runtimeAnimatorController = targetController;

        Debug.Log($"[TowerShooter] state -> {state}");
    }

    /// <summary>
    /// 에디터에서 선택된 상태일 때 공격 범위를 Gizmo로 표시
    /// </summary>
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = (muzzle != null) ? muzzle.position : transform.position;
        Gizmos.DrawWireSphere(origin, range);
    }
#endif
}
