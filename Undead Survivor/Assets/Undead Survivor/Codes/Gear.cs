using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.GearType GearType;
    public float rate;
    float baseMaxHealth; // HealthUp 적용 전 최대 체력 (중복 누적 방지용)


    public void Init(ItemData data)
    {
        // Basic Set
        name = "Gear " + data.itemId;
        transform.parent = GameManager.Instance.player.transform; 
        transform.localPosition = Vector3.zero;

        //Property Set
        GearType = data.gearType;
        rate = data.damages[0]; //Gear의 주요 rate는 damages에 저장 중 (레벨별 누적 상승률)
        baseMaxHealth = GameManager.Instance.maxHealth;

        ApplyGear(); //기어가 처음 생성될 때 로직 적용 함수를 호출
    }
    
    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();    //레벨업 할 때 로직 적용 함수를 호출
    }

    // rate는 해당 레벨의 누적 상승률이므로, 항상 기본값에 한 번만 적용한다.
    // (기존에는 현재 값에 rate를 계속 더해서 무기 획득·레벨업마다 효과가 중복으로 쌓였다.)
    void ApplyGear() //타입에 따라 적절하게 로직을 적용 시켜주는 함수
    {
        switch(GearType)
        {
            case ItemData.GearType.SpeedBoots:
                SpeedUp();
                break;
            case ItemData.GearType.HealthUp:
                HealthUp();
                break;
            case ItemData.GearType.Defense:
                DefenseUp();
                break;
            default: // 나머지는 무기 스탯 패시브 -> 무기마다 스탯을 다시 계산하게 한다
                foreach (Weapon weapon in transform.parent.GetComponentsInChildren<Weapon>())
                    weapon.RefreshStat();
                break;
        }
    }

    // Weapon.RefreshStat에서 호출: 기본 스탯이 복사된 weapon.stat에 이 패시브의 효과를 적용한다.
    public void ApplyTo(Weapon weapon)
    {
        WeaponStat stat = weapon.stat;
        switch (GearType)
        {
            case ItemData.GearType.AsGlove:
                if (weapon.id == 0) // 회전 무기: 회전 속도 증가
                    stat.AttackSpeed = stat.AttackSpeed * Character.WeaponSpeed * (1f + rate);
                else                // 원거리 무기: 공격 간격 감소 (rate는 1보다 작아야 함)
                    stat.AttackSpeed = stat.AttackSpeed * (1f - rate);
                break;
            case ItemData.GearType.AttackUp:
                stat.Damage *= (1f + rate);
                break;
            case ItemData.GearType.AreaRadius:
                stat.AreaRadius *= (1f + rate);
                break;
            case ItemData.GearType.AreaTime:
                stat.Duration *= (1f + rate);
                break;
            case ItemData.GearType.ProjectileCount:
                if (stat.ProjectileNum > 0) // 투사체가 있는 무기만 개수 증가 (기존에는 범위가 늘어나던 버그)
                    stat.ProjectileNum += rate;
                break;
            case ItemData.GearType.ProjectileSpeedUp:
                stat.ProjectileSpeed *= (1f + rate);
                break;
        }
    }

    void SpeedUp()
    {
        float speed = 3 * Character.Speed; //기본 이동속도
        GameManager.Instance.player.speed = speed + speed * rate; //플레이어의 이동속도 증가
    }

    void HealthUp()
    {
        GameManager.Instance.maxHealth = baseMaxHealth * (1f + rate);
    }

    void DefenseUp()
    {
        GameManager.Instance.defense = Character.Defense + rate;
    }
}
