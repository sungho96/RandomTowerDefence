using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Video;
//FSM
public class TowerShooter : MonoBehaviour
{
    [Header("Target Source")]
    // Main/_SceneRoot/Gameplay/Enemies
    [SerializeField] private Transform enemiesparent;

    [Header("Attack Seetings")]
    [SerializeField] private float range = 2.0f; //???????
    [SerializeField] private float hitInterval = 0.8f;
    [SerializeField] private int damage = 1;

    [Header("Optional")]
    [SerializeField] private Transform muzzle; 
    [SerializeField] private bool rotateToTarget = true;

    //FSM ??? 
    [Header("FSM / Anim Controller")]
    [SerializeField] private Animator animator;
    [SerializeField] private RuntimeAnimatorController idleController;
    [SerializeField] private RuntimeAnimatorController attackController;

    [Header("VFX")]
    [SerializeField] private GameObject hitFxPrefab;
    [SerializeField] private float hitFxLifeTime = 1.5f;
    [SerializeField] private float hitFxYOffset = 1.0f;

    [Header("SFX")]
    [SerializeField] private AudioSource hitAudiSource;
    [SerializeField] private AudioClip hitSfx;
    [SerializeField, Range(0f, 1f)] private float hitSfxVolume = 0.8f;
    

    private enum TowerState { Idle, Attack }
    private TowerState state = TowerState.Idle;

    private float nextHitTime = 0f;

    private void Update()
    {
        // GameOver/???????(timeScale=0)?? ?? ??? ??? ???
        if (GameState.Instance != null && GameState.Instance.IsGameOver) return;
        if (Time.timeScale == 0f) return;
        if (enemiesparent == null) return;

        Transform target = FindNearestEnemyInRange();//??? ?? ???

        // target?? null??? Idle?? ?????? return
        if (target == null) { SetState(TowerState.Idle); return; }

        //target?? ?????? Attack ????
        SetState(TowerState.Attack);

        // y??????? ???��? ????? ?????? ???
        if (rotateToTarget)
        {
            Vector3 lookpos = target.position;
            lookpos.y = transform.position.y;
            transform.LookAt(lookpos);
        }
        //??????????? ??????
        if (Time.time >= nextHitTime)
        {
            nextHitTime = Time.time + hitInterval;

            string from = (muzzle != null) ? muzzle.name : gameObject.name;

            EnemyHealth hp = target.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                if (hitFxPrefab != null)
                {
                    Vector3 fxPos = target.position + Vector3.up * hitFxYOffset;

                    Quaternion fxRot = Quaternion.identity;

                    Vector3 dir = (target.position - transform.position);
                    dir.y = -30f;

                    if (dir.sqrMagnitude > 0.001f)
                        fxRot = Quaternion.LookRotation(dir.normalized);

                    GameObject fx = Instantiate(hitFxPrefab, fxPos, fxRot);
                    Destroy(fx, hitFxLifeTime);
                }
                if (hitSfx != null)
                {
                    if (hitAudiSource != null)
                    {
                        hitAudiSource.PlayOneShot(hitSfx, hitSfxVolume);
                    }
                    else
                    {
                        Vector3 sfxPos = (muzzle != null) ? muzzle.position : transform.position;
                        AudioSource.PlayClipAtPoint(hitSfx, sfxPos, hitSfxVolume);
                    } 
                }
            }
            else
                Debug.LogWarning($"[TowerShooter] EnemyHealth not found on{target.name}");

            Debug.Log($"[TowerShooter] hit ({from} -> {target.name})");
        }

    }
    /// <summary>
    /// ???��?(enemiesparent)?? ?????? 
    /// ??????????
    /// EnemyPathFollowes ????????? ???? ?????
    /// ??????? ??? ????? 
    /// ??????? ?? ????
    /// ???? ???????? muzzle ?????? muzzle ???? tower
    /// </summary>
    /// <returns></returns>
    private Transform FindNearestEnemyInRange()
    {
        float rangeSqr = range * range; //sqrt(cpu)???? ?????? ??��????
        Transform best = null;
        float bestDisSqr = float.MaxValue;

        Vector3 origin = (muzzle != null) ? muzzle.position : transform.position;

        for (int i=0; i < enemiesparent.childCount; i++)
        {
            Transform t = enemiesparent.GetChild(i);
            if (!t.gameObject.activeInHierarchy) continue;//???? ????????? ???? 

            // ???? EnemyPathFollowes?? ???? ???????(??? ???)?? ?????? ??? ????
            EnemyPathFollowes follower = t.GetComponentInChildren<EnemyPathFollowes>();//?????????��?? ??? ????????????????
            if (follower == null) continue;

            Transform enemyRoot = follower.transform;

            //orgin???? ???? ???^2
            float dSqr = (enemyRoot.position - origin).sqrMagnitude;

            //???? ?????? ???????? ????
            if (dSqr <= rangeSqr && dSqr < bestDisSqr)
            {
                bestDisSqr = dSqr;
                best = enemyRoot;
            }
        }
        return best;
    }


    /// <summary>
    /// FSM ???? ??? ???
    /// - ???��? ??? ???? ????
    /// - Animator?? ??????, ???��? ?��? RuntimeAnimatorController(idle/attack)?? '???'??? ??? ???? ????.
    /// - ???? ???????? ??? ???????? ????? ?? ?? ???????.
    /// </summary>
    /// <param name="next"></param>
    private void SetState(TowerState next)
    {
        if (state == next) return;
        state = next;

        if (animator == null) return;

        RuntimeAnimatorController targetController =
            (state == TowerState.Attack) ? attackController : idleController;

        if (targetController != null && animator.runtimeAnimatorController != targetController)//???? Animator?? ??? ???? ??????(???��??/??? ????)
            animator.runtimeAnimatorController = targetController;

        Debug.Log($"[TowerShooter] state -> {state}");
    }

    /// <summary>
    /// ?????????? ??????? ???????? ???
    /// </summary>
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = (muzzle != null) ? muzzle.position : transform.position;
        Gizmos.DrawWireSphere(origin, range);
    }
#endif
}