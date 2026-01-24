using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//콜라이더
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 10;
    [SerializeField] private int currentHp;

    [Header("Death FX")]
    [SerializeField] private float deathDuration = 0.45f;
    [SerializeField] private float tiltAngle = 85f;
    [SerializeField] private float ShrinkScale = 0.5f;
    [SerializeField] private bool disableMoveMentOnDeath = true;

    private bool isDying = false;
    public int CurrentHp => currentHp; //캡슐화

    private void Awake()
    {
        currentHp = maxHp;
    }

    /// <summary>
    /// amount = 받을데미지
    /// 죽는 중 & 현재체력 0이하면 return;
    /// current는 0이하로 안떨어지게끔 조정
    /// current=0 이면 Die() 호출
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        if (isDying) return;
        if (currentHp <= 0) return;

        currentHp = Mathf.Max(0, currentHp-amount); //0미만방지
        Debug.Log($"[EnemyHealth] {name} HP : {currentHp}/{maxHp}");

        if (currentHp == 0)
            Die();
    }

    /// <summary>
    /// 죽는중이면 return
    /// 아니면 죽는 object  pathfollowe 가져와서 해당 Script 비활성화
    /// DieRoutine() 호출
    /// </summary>
    private void Die()
    {
        if (isDying) return; //죽는것 중복방지
        isDying = true;

        Debug.Log($"[EnemyHealth] {name} Dead");

        if(disableMoveMentOnDeath)
        {
            //Enemypathfollowes를 멈추면 "쓰러지는 동안" 이동하지 않음
            EnemyPathFollowes follow = GetComponentInChildren<EnemyPathFollowes>();
            if (follow != null) follow.enabled = false;
        }

        StartCoroutine(DieRoutine());
    }

    /// <summary>
    /// 호출되면 크기와 Rot 가져오기
    /// 죽는동안 콜라이더 무시
    /// 시간에 맞게 크기가줄어들면서 z축방향으로 돌아가면서(clamp01) 진행
    /// 끝나면 해당오브젝트 제거
    /// </summary>
    /// <returns></returns>
    private IEnumerator DieRoutine()
    {
        float t = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * ShrinkScale;

        Quaternion startRot =  transform.rotation;
        //옆으로 넘어지게: 로컬 축 기준으로 z축으로 기울이기
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, tiltAngle);

        //죽는동안 콜라이더 무시
        Collider col = GetComponentInChildren<Collider>();
        if (col != null) col.enabled = false;

        while (t < deathDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / deathDuration); //퍼센트 비율쓰기위해 사용

            transform.rotation = Quaternion.Slerp(startRot, endRot, a);
            transform.localScale = Vector3.Lerp(startScale, endScale, a);

            yield return null;
        }

        Destroy(gameObject);

    }
}
