using UnityEngine;
[System.Serializable]
public class WeaponStat
{
    public float Damage;
    public float AttackSpeed;
    public float AreaRadius;
    public float Duration;
    public float ProjectileNum;
    public float ProjectileSpeed;
    public float count;
    public void init(ItemData data)
    {
        Damage = data.stat.Damage;
        AttackSpeed = data.stat.AttackSpeed;
        AreaRadius = data.stat.AreaRadius;
        Duration = data.stat.Duration;
        ProjectileNum = data.stat.ProjectileNum;
        ProjectileSpeed = data.stat.ProjectileSpeed;
        count = data.stat.count;
    }
}
