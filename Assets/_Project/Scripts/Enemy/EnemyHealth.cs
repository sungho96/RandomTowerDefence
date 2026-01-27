using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//�ݶ��̴�
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
    public int CurrentHp => currentHp; //ĸ��ȭ

    private void Awake()
    {
        currentHp = maxHp;
        UpdateHpText();
    }

    /// <summary>
    /// amount = ����������
    /// �״� �� & ����ü�� 0���ϸ� return;
    /// current�� 0���Ϸ� �ȶ������Բ� ����
    /// current=0 �̸� Die() ȣ��
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        if (isDying) return;
        if (currentHp <= 0) return;

        currentHp = Mathf.Max(0, currentHp - amount); //0�̸�����
        UpdateHpText();
        Debug.Log($"[EnemyHealth] {name} HP : {currentHp}/{maxHp}");

        if (currentHp == 0)
            Die();
    }

    /// <summary>
    /// �״����̸� return
    /// �ƴϸ� �״� object  pathfollowe �����ͼ� �ش� Script ��Ȱ��ȭ
    /// DieRoutine() ȣ��
    /// </summary>
    private void Die()
    {
        if (isDying) return; //�״°� �ߺ�����
        isDying = true;

        Debug.Log($"[EnemyHealth] {name} Dead");

        if(disableMoveMentOnDeath)
        {
            //Enemypathfollowes�� ���߸� "�������� ����" �̵����� ����
            EnemyPathFollowes follow = GetComponentInChildren<EnemyPathFollowes>();
            if (follow != null) follow.enabled = false;
        }

        StartCoroutine(DieRoutine());
    }

    /// <summary>
    /// ȣ��Ǹ� ũ��� Rot ��������
    /// �״µ��� �ݶ��̴� ����
    /// �ð��� �°� ũ�Ⱑ�پ��鼭 z��������� ���ư��鼭(clamp01) ����
    /// ������ �ش������Ʈ ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator DieRoutine()
    {
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
        //������ �Ѿ�����: ���� �� �������� z������ ����̱�
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, tiltAngle);

        //�״µ��� �ݶ��̴� ����
        Collider col = GetComponentInChildren<Collider>();
        if (col != null) col.enabled = false;

        while (t < deathDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / deathDuration); //�ۼ�Ʈ ������������ ���

            transform.rotation = Quaternion.Slerp(startRot, endRot, a);
            transform.localScale = Vector3.Lerp(startScale, endScale, a);

            yield return null;
        }

        Destroy(gameObject);

    }
    
    private void UpdateHpText()
    {
        if (hpText == null) return;
        hpText.text = $"{currentHp}/{maxHp}";
    }
}
