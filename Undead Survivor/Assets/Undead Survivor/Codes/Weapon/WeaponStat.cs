using UnityEngine;
[System.Serializable]
public class WeaponStat
{
    // 무기 스탯 상한 (레벨업·진화·패시브를 모두 적용한 최종값 기준)
    public const float MaxProjectileSpeed = 8f;   // 투사체 속도 최대 8
    public const float MinAttackInterval = 0.5f;  // 공격 간격은 0.5초보다 짧아지지 않음
    public const float MinProjectileInterval = 1.5f; // Projectile류(투사체 장판, 투사체 폭탄)는 공격 간격 1.5초 이상
    public const float MaxAreaRadius = 4f;        // 범위 최대 4
    public const float DurationRatio = 0.9f;      // 유지 시간은 공격 간격의 90%까지 (공격 간격보다 짧게)

    public float Damage;
    public float AttackSpeed;
    public float AreaRadius;
    public float Duration;
    public float ProjectileNum;
    public float ProjectileSpeed;
    public void init(ItemData data)
    {
        Damage = data.stat.Damage * Character.Damage;
        AttackSpeed = data.stat.AttackSpeed;
        AreaRadius = data.stat.AreaRadius;
        Duration = data.stat.Duration;
        ProjectileNum = data.stat.ProjectileNum;
        ProjectileSpeed = data.stat.ProjectileSpeed;
    }

    // 투사체들이 같은 WeaponStat 참조를 들고 있으므로 새로 만들지 않고 값만 복사한다.
    public void CopyFrom(WeaponStat other)
    {
        Damage = other.Damage;
        AttackSpeed = other.AttackSpeed;
        AreaRadius = other.AreaRadius;
        Duration = other.Duration;
        ProjectileNum = other.ProjectileNum;
        ProjectileSpeed = other.ProjectileSpeed;
    }

    // minInterval: 공격 간격 하한 (기본 0.5초, Projectile류는 1.5초)
    // isRotation: 회전 무기(id 0)는 AttackSpeed가 공격 간격이 아니라 초당 회전 각도이므로 간격 제한을 적용하지 않는다.
    public void ClampLimits(bool isRotation, float minInterval = MinAttackInterval)
    {
        ProjectileSpeed = Mathf.Min(ProjectileSpeed, MaxProjectileSpeed);
        AreaRadius = Mathf.Min(AreaRadius, MaxAreaRadius);

        if (isRotation || AttackSpeed <= 0)
            return;

        AttackSpeed = Mathf.Max(AttackSpeed, minInterval);
        if (Duration > 0)
            Duration = Mathf.Min(Duration, AttackSpeed * DurationRatio);
    }
}
