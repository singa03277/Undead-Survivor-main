using Cinemachine.Utility;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
//using static UnityEngine.RuleTile.TilingRuleOutput;


public class Weapon : MonoBehaviour //���� ������ ���� ��ũ��Ʈ(������ id�� ���� �ٸ� �۵��� �����ش�)
{
    public int prefabId;
    public int id;
    public bool isKnockBack = false;
    private bool isRandom = false;
    private Vector3 lastDir;
    private float SequenceCount;
    private bool isEvolved = false;
    private bool isMaxLevel = false;
    Player player;
    public WeaponStat stat = new WeaponStat();
    ItemData data;
    
    void Awake()
    {
        player = GameManager.Instance.player;
    }

    void Update()
    {
        if (!GameManager.Instance.isLive)
            return;

        
        switch (data.itemId)
        {
            case 0:
                transform.Rotate(Vector3.back * stat.AttackSpeed * Time.deltaTime);
                break;
            default:
                break;

        }

    }

    public void Init(ItemData data)
    {
        this.data = data;
        name = "Weapon " + data.itemId; //�̸� ����
        transform.parent = player.transform; //�θ� ������Ʈ ����
        transform.localPosition = Vector3.zero;


        id = data.itemId;
        stat.init(data);

        for (int i = 0; i < GameManager.Instance.pool.prefabs.Length; i++)
        {

            if (data.projectile == GameManager.Instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        switch (data.itemId)
        {
            case 0:
                Batch();
                break;
            case 3:
                SpawnArea();
                break;
            case 7:
                SpawnLaser();
                break;
            case 8:
                SpawnFront();
                break;
            case 9:
                StartCoroutine(BackDotRoutine());
                break;
            default:
                StartCoroutine(ShootRoutine());
                break;
        }
        Hand hand = player.hands[(int)data.itemType]; //������ Ÿ�Կ� �´� �ڵ带 �������� hand�� ����
        hand.spriter.sprite = data.hand; //�߰��س��� hand sprite�� ����
        hand.gameObject.SetActive(true);

        //Weapon�� ���Ӱ� �߰��Ǹ� ApplyGear�� ���Ӱ� �߰��� ���⿡ Gear ������ ����
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); //�÷��̾�� broadcast���ֵ��� ��Ź

    }

    void FireRange(string name) // ���Ÿ� �߻翡 ���� �Լ�
    {
        Vector3 dir;
        if (data.isRandomPlace)
        {
            dir = player.scanner.randomTarget == null ? lastDir : CalcuDistance(player.scanner.randomTarget);
        }
        else
        {
            dir = player.scanner.nearestTarget == null ? lastDir : player.scanner.randomTarget.normalized;

        }
        lastDir = dir;

        Transform Range = GameManager.Instance.pool.Get(prefabId).transform;
        Range.position = transform.position;
        Range.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        switch (name)
        {
            case "Bullet":
                Range.GetComponent<Bullet>().init(stat, dir, data.isKnockback);
                break;
            case "SequenceBullet":
                Range.GetComponent<Bullet>().init(stat, dir, data.isKnockback);
                break;
        }

    }

    Vector3 CalcuDistance(Vector3 dest)
    {
        if (dest == null)
            return Vector3.zero;

        Vector3 dir = dest - transform.position;
        return dir.normalized;
    }

    public void LevelUp(float damage, int level) //���Ÿ� �߻�ü�� �ƴѰ��� �߰����� �Լ� ���� �ʿ�(spawn --- �Լ��� �����ʿ�)
    {
        stat.Damage = damage * Character.Damage;

        switch (data.LevelUpStatTypes[level])
        {
            case ItemData.SubStatType.AttackSpeed:
                stat.AttackSpeed += (stat.AttackSpeed * data.subStat[level]);
                break;
            case ItemData.SubStatType.AreaRadius:
                stat.AreaRadius += (stat.AreaRadius * data.subStat[level]);
                break;
            case ItemData.SubStatType.AreaDuration:
                stat.Duration += (stat.Duration * data.subStat[level]);
                break;
            case ItemData.SubStatType.ProjectileNum:
                stat.ProjectileNum += data.subStat[level];
                break;
            case ItemData.SubStatType.ProjectileSpeed:
                stat.ProjectileSpeed += (stat.ProjectileSpeed * data.subStat[level]);
                break;
        }

        if (id == 0)
            Batch();

        if(isMaxLevel == false && level+1 == data.damages.Length)
            isMaxLevel = true;

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    

    void Batch()// ������ ���⸦ ��ġ�ϴ� �Լ�
    {
        for (int i = 0; i < stat.ProjectileNum; i++)
        {

            Transform bullet;
            
            // ���� ������Ʈ�� ���� Ȱ���ϰ� ���ڶ� ���� Ǯ������ ��������
            if (i < transform.childCount) // �ڽ��� ������ ������ ���� ������ �ʰ�
            {
                bullet = transform.GetChild(i);  //������ �ڽĵ��� ������ ����.
            }
            else
            {
                bullet = GameManager.Instance.pool.Get(prefabId).transform;
                bullet.parent = transform;  //���� �������� �͵鸸 parent�� �������ָ� �ȴ�. 
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * i / stat.ProjectileNum; //i��° ������ ȸ�� ������ ���
            bullet.Rotate(rotVec);                              //rotVec��ŭ ȸ��
             
            //�̵� ������ Space.self�� �ƴ϶� World�� ������? �̹� ȸ�� �� ���� �������� 1.5��ŭ �̵���Ű�� ������ �����Ƿ� �̵� ������ ���带 �������� ����
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<RotationWeapon>().init(stat,data.isKnockback);
        }
    }
    void SpawnLaser()
    {
        Transform Laser;
        Laser = GameManager.Instance.pool.Get(prefabId).transform;
        Laser.parent = transform;

        Laser.localPosition = Vector3.zero;
        Laser.localRotation = Quaternion.identity;

        Laser.GetComponent<Laser>().init(stat,data.isKnockback,isEvolved);
    }

    void SpawnFront()
    {

        Transform front = GameManager.Instance.pool.Get(prefabId).transform;
        front.parent = transform;
        front.localPosition = Vector3.zero;
        front.localRotation = Quaternion.identity;
        
        if (isEvolved) 
        {
            front.GetComponent<FrontAttack>().init(stat, data.isKnockback, true);
            return;
        }
        else
            front.GetComponent<FrontAttack>().init(stat, data.isKnockback, false);
    }

    void SpawnArea()
    {
        Transform Area;

        Area = GameManager.Instance.pool.Get(prefabId).transform;
        Area.parent = transform;

        Area.localPosition = Vector3.zero;
        Area.localRotation = Quaternion.identity;
        Area.GetComponent<SlowArea>().init(stat,isEvolved);
    }



    IEnumerator ShootBackDot()
    {
        while (true)
        {
            Transform BackDotObject = GameManager.Instance.pool.Get(12).transform;
            Vector3 dir = player.scanner.randomTarget;
            Debug.Log(dir);
            BackDotObject.position = transform.position;
            BackDotObject.rotation = Quaternion.identity;
            BackDotObject.GetComponent<EvolvedBackDot>().init(stat, CalcuDistance(player.scanner.randomTarget), data.isKnockback);
            yield return new WaitForSeconds(stat.AttackSpeed);
        }
    }

    IEnumerator BackDotRoutine()
    {
        while (true)
        {
            SpawnBackDot();
            yield return new WaitForSeconds(stat.AttackSpeed);
        }
    }

    void SpawnBackDot()
    {
        Transform Dot = GameManager.Instance.pool.Get(prefabId).transform;

        Dot.position = transform.position;
        Dot.rotation = Quaternion.identity;
        Dot.GetComponent<Dot>().init(stat, data.isKnockback, isEvolved);
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            Transform bullet;
            Vector3 dir;
            if (prefabId == 5)
                dir = player.scanner.randomTarget;
            else
                dir = player.scanner.nearestTarget.position;
            for (int i = 0; i < stat.ProjectileNum; i++) 
            {
                bullet = GameManager.Instance.pool.Get(prefabId).transform;
                bullet.position = transform.position;
                bullet.rotation = Quaternion.identity;
                switch (prefabId)
                {
                    case 1:
                        bullet.GetComponent<Bullet>().init(stat, dir, data.isKnockback);
                        break;
                    case 2:
                        bullet.GetComponent<Projectile>().init(stat, Random.Range(8f, 11f), dir, data.isKnockback, isEvolved);
                        break;
                    case 4:
                        bullet.GetComponent<Projectile>().init(stat, Random.Range(8f, 11f), dir, data.isKnockback, isEvolved);
                        break;
                    case 5:
                        bullet.GetComponent<Projectile>().init(stat, Random.Range(8f, 11f), dir, data.isKnockback, isEvolved);
                        break;
                    case 6:
                        bullet.GetComponent<Boundingweapon>().init(stat, dir, data.isKnockback, isEvolved);
                        break;
                    case 10:
                        bullet.GetComponent<Boomerang>().init(stat, dir, data.isKnockback, isEvolved);
                        break;
                }
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(stat.AttackSpeed);
        }
    }

    public void CheckEvolve()
    {
        if (isEvolved)
            return;

        if (!isMaxLevel)
            return;

        if (!GameManager.Instance.passiveInventory.Contains(data.evolvePassiveId))
            return;

        isEvolved = true;
        ApplyEvolve();
    }

    void ApplyEvolve()
    {
        data = data.evolveData;
        stat.init(data);
        if (id == 8)
            SpawnFront();
        if (id == 9)
            StartCoroutine(ShootBackDot());

    }
}
