using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.GearType GearType;
    public float rate;


    public void Init(ItemData data)
    {
        // Basic Set
        name = "Gear " + data.itemId;
        transform.parent = GameManager.Instance.player.transform; 
        transform.localPosition = Vector3.zero;

        //Property Set
        GearType = data.gearType;
        rate = data.damages[0]; //Gear의 주요 rate는 damages에 저장 중

        ApplyGear(); //기어가 처음 생성될 때 로직 적용 함수를 호출
    }
    
    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();    //레벨업 할 때 로직 적용 함수를 호출
    }

    void ApplyGear() //타입에 따라 적절하게 로직을 적용 시켜주는 함수 추가
    {
        switch(GearType)
        {
            case ItemData.GearType.AsGlove:
                RateUp();
                break;
            case ItemData.GearType.SpeedBoots:
                SpeedUp();
                break;
            case ItemData.GearType.HealthUp:
                HealthUp();
                break;
            case ItemData.GearType.Defense:
                break;
            case ItemData.GearType.AttackUp:
                break;
            case ItemData.GearType.AreaRadius:
                break;
            case ItemData.GearType.AreaTime:
                break;
            case ItemData.GearType.ProjectileCount:
                break;
            case ItemData.GearType.ProjectileSpeedUp:
                break;
        }
    }

    void RateUp() //장갑의 기능인 연사력을 올리는 함수
    {
        // 부모로 올라가서 자식들의 Weapon 컴포넌트들을 다 가져온다.
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        //무기 하나씩 순회하면서 타입에 따라 speed 값 변경
        foreach(Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0: //근거리 무기의 경우
                    float speed = 150 * Character.WeaponSpeed;
                    weapon.stat.AttackSpeed = speed + (speed * rate); // 회전 속도를 증가
                    break;
                default: //원거리 무기의 경우
                    weapon.stat.AttackSpeed = weapon.stat.AttackSpeed * (1f - rate);  // 무조건 공격속도 테이블에서는 소수점으로 줘야함 - 그러지 않으면 -로 넘어가게 됨. 
                    break;
            }

        }
    }

    void SpeedUp()
    {
        float speed = 3 * Character.Speed; //기본 이동속도
        GameManager.Instance.player.speed = speed + speed * rate; //플레이어의 이동속도 증가
    }

    void HealthUp()
    {
        GameManager.Instance.maxHealth += (GameManager.Instance.maxHealth * rate);
    }

    void DefenseUp()
    {

    }
}
