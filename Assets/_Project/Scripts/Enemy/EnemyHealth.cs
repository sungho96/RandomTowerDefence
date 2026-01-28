using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 10;
    [SerializeField] private int currentHp;

    [Header("Death VFX")]
    [SerializeField] private float deathDuration = 0.45f;
    [SerializeField] private float tiltAngle = 85f;
    [SerializeField] private float ShrinkScale = 0.5f;
    [SerializeField] private bool disableMoveMentOnDeath = true;

    [Header("Death SFX")]
    [SerializeField] private AudioSource DeathAudioSource;
    [SerializeField] private AudioClip DeathSfx;
    [SerializeField, Range(0f, 1f)] private float DeathSoundVolume = 0.5f;

    [Header("HP UI")]
    [SerializeField] private TextMeshPro hpText;

    private bool isDying = false;
    public int CurrentHp => currentHp; // 캡슐화(읽기 전용)

    private void Awake()
    {
        currentHp = maxHp;
        UpdateHpText();
    }

    /// <summary>
    /// 데미지 적용
    /// - 이미 죽는 중이면 무시
    /// - HP가 0 이하면 무시
    /// - HP는 0 아래로 내려가지 않도록 Clamp
    /// - HP가 0이 되면 Die() 호출
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (isDying) return;
        if (currentHp <= 0) return;

        currentHp = Mathf.Max(0, currentHp - amount); // 0 아래로 내려가지 않게 처리
        UpdateHpText();
        Debug.Log($"[EnemyHealth] {name} HP : {currentHp}/{maxHp}");

        if (currentHp == 0)
            Die();
    }

    /// <summary>
    /// 사망 처리 시작
    /// - 중복 사망 방지(isDying)
    /// - 옵션에 따라 이동 스크립트(EnemyPathFollowes) 비활성화
    /// - DieRoutine() 코루틴 시작
    /// </summary>
    private void Die()
    {
        if (isDying) return; // 사망 처리 중복 방지
        isDying = true;

        Debug.Log($"[EnemyHealth] {name} Dead");

        if (disableMoveMentOnDeath)
        {
            // EnemyPathFollowes를 꺼서 "죽는 연출 동안" 이동을 멈춤
            EnemyPathFollowes follow = GetComponentInChildren<EnemyPathFollowes>();
            if (follow != null) follow.enabled = false;
        }

        StartCoroutine(DieRoutine());
    }

    /// <summary>
    /// 사망 연출 코루틴
    /// - SFX 재생
    /// - Collider 비활성화(죽은 뒤 충돌/타격 방지)
    /// - deathDuration 동안:
    ///   회전(기울기) + 스케일(축소) 
    /// - 연출 종료 후 Destroy
    /// </summary>
    private IEnumerator DieRoutine()
    {
        // Death SFX
        if (DeathSfx != null)
        {
            if (DeathAudioSource != null)
            {
                DeathAudioSource.PlayOneShot(DeathSfx, DeathSoundVolume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(DeathSfx, transform.position, DeathSoundVolume);
            }
        }

        float t = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * ShrinkScale;

        Quaternion startRot = transform.rotation;
        // 쓰러지는 방향: Z축으로 기울이기
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, tiltAngle);

        // 죽는 동안 충돌 비활성화
        Collider col = GetComponentInChildren<Collider>();
        if (col != null) col.enabled = false;

        while (t < deathDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / deathDuration);

            transform.rotation = Quaternion.Slerp(startRot, endRot, a);
            transform.localScale = Vector3.Lerp(startScale, endScale, a);

            yield return null;
        }

        Destroy(gameObject);
    }
    /// <summary>
    /// 적 HP를 UI 등 전송하기위한 함수
    /// </summary>
    private void UpdateHpText()
    {
        if (hpText == null) return;
        hpText.text = $"{currentHp}/{maxHp}";
    }
}
