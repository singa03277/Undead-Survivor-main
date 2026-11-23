﻿using UnityEngine;

public class SlowArea : MonoBehaviour
{
    const float MaxSlowPer = 0.7f; // 둔화율 상한(1 이상이면 적의 속도가 0 이하가 되어 뒤로 움직임)

    float stunTimer = 0f;          // 진화 시 기절 주기 타이머
    public float slowPer;
    bool isEvolved = false;

    WeaponStat stat;
    CircleCollider2D cirCollider;

    private void Awake()
    {
        cirCollider = GetComponent<CircleCollider2D>();
    }

    public void init(WeaponStat stat, bool isEvolved)
    {
        this.stat = stat;
        this.isEvolved = isEvolved;
        stunTimer = 0f;
        ApplyStat();
    }

    // 레벨업·장비로 바뀐 범위와 둔화율이 반영되도록 stat 값을 다시 적용한다.
    void ApplyStat()
    {
        cirCollider.radius = stat.AreaRadius;
        slowPer = Mathf.Clamp(stat.Damage, 0f, MaxSlowPer);
    }

    private void FixedUpdate()
    {
        if (stat == null)
            return;

        ApplyStat();

        if (!isEvolved)
            return;

        // 진화: AttackSpeed(초)마다 범위 안의 적을 Duration(초) 동안 기절시킨다.
        // (기존에는 타이머 변수가 뒤바뀌어 매 프레임 기절 코루틴이 실행되었음)
        stunTimer += Time.fixedDeltaTime;
        if (stunTimer < stat.AttackSpeed)
            return;

        stunTimer = 0f;
        RaycastHit2D[] Enemys = Physics2D.CircleCastAll(transform.position, stat.AreaRadius, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
        foreach (RaycastHit2D scanEnemy in Enemys)
        {
            EnemyStatus enemy = scanEnemy.collider.GetComponent<EnemyStatus>();
            if (enemy != null)
                enemy.stun(stat.Duration);
        }
    }
}
