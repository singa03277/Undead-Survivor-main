using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item",menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    //무기 타입 : 근거리, 원거리, 장갑, 신발, 힐
    public enum ItemType { Melee, Range, Passive, Heal }
    public enum GearType { AsGlove, SpeedBoots, HealthUp, Defense, AttackUp, AreaRadius, AreaTime, ProjectileCount, ProjectileSpeedUp, None }
    public enum SubStatType { AttackSpeed , AreaRadius, AreaDuration, ProjectileNum, ProjectileSpeed, count, none }

    [Header("# Basic Info")]
    public WeaponStat stat;

    // 아이템의 각종 속성들을 변수로 작성(count는 서브 능력치)
    [Header("# Main Info ")]
    public ItemType itemType;
    public GearType gearType;

    public int itemId;
    public string itemName;
    public bool isRandomPlace;
    public bool isKnockback;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;

    [Header("# Level Data")]  
    public float[] damages; 
    public int[] subStat;    
    public SubStatType[] LevelUpStatTypes;

    [Header("# Weapon")]
    public GameObject projectile;
    public Sprite hand;

    [Header("# EvolveItemData")]
    public ItemData evolveData;
}
