using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;   //������ ������
    public int level;       //���� 
    public Weapon weapon;   //���� 
    public Gear gear;       //��� 
    public bool isConsumable;
    

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
        //�ڽ� ������Ʈ icon
        icon = GetComponentsInChildren<Image>()[1]; //ù��°[0]�� �ڱ��ڽ�
        icon.sprite = data.itemIcon;                //itemData�� ���������� �ʱ�ȭ

        Text[] texts = GetComponentsInChildren<Text>(); //�ڽ��� Text ������Ʈ ��������
        textLevel = texts[0];   //item ������Ʈ���� Text�� ��� �ڽĿ� �ִ� Text�� ���� ������ ù��°[0]�� �ʱ�ȭ
        textName = texts[1];    //GetComponents�� ������ ���� ������ ������ ���󰣴�.
        textDesc = texts[2];
        textName.text = data.itemName; //������ �̸� �����ε� ��ư�� �����̹Ƿ� �ٷ� �ʱ�ȭ
    }

    void OnEnable()
    {
        textLevel.text = "Lv." + (level+1);
        switch (data.itemType)
        {
            //%�� �׻� 100�� ���ؼ� �Ѱ��ֱ�
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if(level == 0)
                    textDesc.text = data.itemDesc;
                else
                    textDesc.text = GetItemText(level);
                break;
            case ItemData.ItemType.Passive:
                //���� �Էµ� itemDesc�� �ؽ�Ʈ�� �Ű������� damages�� counts�� �־ ���ڿ� ����
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            default:
                //����� ���� ���
                textDesc.text = string.Format(data.itemDesc, data.damages[level]);
                break;

        }
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0) //������ 0�� �� ��ư�� ������ ���� ������Ʈ�� ����
                {
                    GameObject newWeapon = new GameObject();
                    GameManager.Instance.weaponInventory.Add(data.itemId);
                    if(GameManager.Instance.weaponInventory.Count == 6)
                    {
                        // 같은 리스트를 공유하면 weaponPool.Remove가 인벤토리에서도 지워지므로 복사본을 쓴다.
                        GameManager.Instance.weaponPool = new List<int>(GameManager.Instance.weaponInventory);
                    }
                        
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.stat.Damage;

                    nextDamage += data.stat.Damage * data.damages[level-1]; //damages�� ������̱� ������ ���ؼ� ������

                    weapon.LevelUp(nextDamage, level-1); //Weapon�� LevelUp �Լ��� �̿��� ������
                }
                level++;
                if (level-1 == data.damages.Length)
                {
                    GameManager.Instance.weaponPool.Remove(data.itemId);
                    weapon.CheckEvolve();
                }
                break;
                        
            case ItemData.ItemType.Passive:
                if (level == 0) //������ 0�� �� ��ư�� ������ ��� ������Ʈ�� ����
                {   
                    GameObject newGear = new GameObject();
                    GameManager.Instance.passiveInventory.Add(data.itemId);
                    if (GameManager.Instance.passiveInventory.Count == 6)
                    {
                        // 같은 리스트를 공유하면 passivePool.Remove가 passiveInventory에서도 지워져 진화 조건 검사가 실패한다.
                        GameManager.Instance.passivePool = new List<int>(GameManager.Instance.passiveInventory);
                    }
                        
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];

                    gear.LevelUp(nextRate);
                }
                level++;

                Weapon[] weapons = GameManager.Instance.player.GetComponentsInChildren<Weapon>();

                foreach (Weapon weapon in weapons)
                {
                    weapon.CheckEvolve();
                }

                // 패시브는 damages[0]이 Lv1이라 레벨 수 = damages.Length.
                // (기존 level-1 조건이면 Lv6을 고를 때 damages[5]를 읽어 IndexOutOfRange 오류 발생)
                if (level == data.damages.Length)
                {
                    GameManager.Instance.passivePool.Remove(data.itemId);
                }

                break;
            case ItemData.ItemType.Heal: // ��ȸ�� �������� �ȿ����� LevelUp �Լ� ������ X
                GameManager.Instance.health = GameManager.Instance.maxHealth;
                break;
        }
    }

    string GetItemText(int level)
    {
        string itemText = "";

        if (level == 1 && data.damages[level - 1] != data.stat.Damage)
            itemText += $"\n데미지 {data.damages[level - 1] * 100}% 증가";
        else if(level >= 2 && data.damages[level-1] != data.damages[level-2])
            itemText += $"\n데미지 {data.damages[level - 1] * 100}% 증가";


        switch (data.LevelUpStatTypes[level-1])
        {
            // subStat은 비율(0.2 = 20%)로 저장되어 있으므로 100을 곱해서 표시한다.
            case ItemData.SubStatType.AttackSpeed:
                float rate = data.subStat[level - 1] * 100;
                if (rate < 0) // 원거리 무기는 AttackSpeed가 공격 간격이므로 음수 = 더 빨라짐
                    itemText += $"\n공격 간격 {-rate}% 감소";
                else
                    itemText += $"\n공격속도 {rate}% 증가";
                break;
            case ItemData.SubStatType.AreaRadius:
                itemText += $"\n범위공격 범위 {data.subStat[level - 1] * 100}% 증가";
                break;
            case ItemData.SubStatType.ProjectileNum:
                itemText += $"\n투사체 개수 {data.subStat[level - 1]}개 증가";
                break;
            case ItemData.SubStatType.ProjectileSpeed:
                itemText += $"\n투사체 속도 {data.subStat[level - 1] * 100}% 증가";
                break;
            case ItemData.SubStatType.AreaDuration:
                itemText += $"\n범위공격 유지 시간 {data.subStat[level - 1] * 100}% 증가";
                break;
            case ItemData.SubStatType.none:
                break;
        }

        return itemText;
    }
}
