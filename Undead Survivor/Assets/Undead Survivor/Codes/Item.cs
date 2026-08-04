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
        textLevel.text = "Lv." + (level + 1);
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
            case ItemData.ItemType.weapon:
                if (level == 0) //������ 0�� �� ��ư�� ������ ���� ������Ʈ�� ����
                {
                    GameObject newWeapon = new GameObject();

                    //���ο� ������Ʈ�� Weapon ������Ʈ �߰�
                    //AddComponent �Լ� ��ȯ ���� �̸� ������ ������ ����.
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
                        
                }
                
                level++;
                break;
            case ItemData.ItemType.Passive:
                if (level == 0) //������ 0�� �� ��ư�� ������ ��� ������Ʈ�� ����
                {
                    GameObject newGear = new GameObject();

                    //���ο� ������Ʈ�� Weapon ������Ʈ �߰�
                    //AddComponent �Լ� ��ȯ ���� �̸� ������ ������ ����.
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];

                    gear.LevelUp(nextRate);
                }
                level++;
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
            itemText += $"\n���ݷ� {data.damages[level - 1] * 100}% ����";
        else if(level >= 2 && data.damages[level-1] != data.damages[level-2])
            itemText += $"\n���ݷ� {data.damages[level - 1] * 100}% ����";


        switch (data.LevelUpStatTypes[level])
        {
            case ItemData.SubStatType.AttackSpeed:
                itemText += $"\n���ݼӵ� {data.subStat[level-1]}% ����";
                break;
            case ItemData.SubStatType.AreaRadius:
                itemText += $"\n���� {data.subStat[level-1]}% ����";
                break;
            case ItemData.SubStatType.ProjectileNum:
                itemText += $"\n����ü {data.subStat[level - 1]}�� ����";
                break;
            case ItemData.SubStatType.ProjectileSpeed:
                itemText += $"\n����ü �ӵ� {data.subStat[level - 1]}% ����";
                break;
            case ItemData.SubStatType.AreaDuration:
                itemText += $"\n���� �ð� {data.subStat[level - 1]}% ����";
                break;
            case ItemData.SubStatType.count: // id�� ��� �ٸ��� ���� ������ �����ϱ�
                itemText += $"\nī��Ʈ {data.subStat[level - 1]} ����";
                break;
            case ItemData.SubStatType.none:
                break;
        }

        return itemText;
    }
}
