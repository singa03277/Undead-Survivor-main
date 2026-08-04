using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item",menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    //���� Ÿ�� : �ٰŸ�, ���Ÿ�, �尩, �Ź�, ��
    public enum ItemType { Melee, Range, Passive, Heal }
    public enum GearType { AsGlove, SpeedBoots, HealthUp, Defense, AttackUp, AreaRadius, AreaTime, ProjectileCount, ProjectileSpeedUp, None }
    public enum SubStatType { AttackSpeed , AreaRadius, AreaDuration, ProjectileNum, ProjectileSpeed, count, none }

    [Header("# Basic Info")]
    public WeaponStat stat;

    // �������� ���� �Ӽ����� ������ �ۼ�(count�� ���� �ɷ�ġ)
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
