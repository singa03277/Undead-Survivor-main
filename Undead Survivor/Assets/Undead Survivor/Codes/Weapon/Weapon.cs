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
    public WeaponStat stat = new WeaponStat();      // 패시브와 상한까지 적용된 최종 스탯 (투사체들이 이 참조를 사용)
    WeaponStat baseStat = new WeaponStat();         // 무기 데이터 + 레벨업만 반영한 스탯 (패시브 미적용)
    ItemData data;
    
    void Awake()
    {
        player = GameManager.Instance.player;
    }

    void Update()
    {
        if (!GameManager.Instance.isLive)
            return;

        if(data.itemId == 0)
            transform.Rotate(Vector3.back * stat.AttackSpeed * Time.deltaTime);

    }

    public void Init(ItemData data)
    {
        this.data = data;
        name = "Weapon " + data.itemId; //�̸� ����
        transform.parent = player.transform; //�θ� ������Ʈ ����
        transform.localPosition = Vector3.zero;
        

        id = data.itemId;

        for (int i = 0; i < GameManager.Instance.pool.prefabs.Length; i++)
        {

            if (data.projectile == GameManager.Instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        baseStat.init(data);
        RefreshStat(); // 이미 가지고 있는 패시브를 새 무기에 적용

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
    }

    // 기본 스탯(데이터 + 레벨업)에 보유한 패시브를 한 번씩만 적용하고 상한을 건다.
    // 매번 기본값에서 다시 계산하므로 패시브 효과가 중복으로 누적되지 않는다.
    public void RefreshStat()
    {
        stat.CopyFrom(baseStat);

        foreach (Gear gear in player.GetComponentsInChildren<Gear>())
            gear.ApplyTo(this);

        bool isProjectile = id == 2 || id == 5; // Projectile.cs를 쓰는 투사체 장판(2), 투사체 폭탄(5)
        stat.ClampLimits(id == 0, isProjectile ? WeaponStat.MinProjectileInterval : WeaponStat.MinAttackInterval);

        if (id == 0 && transform.childCount > 0) // 회전 무기는 개수가 바뀌었을 수 있으니 재배치 (최초 배치는 Init에서)
            Batch();
    }

    void FireRange(string name) // ���Ÿ� �߻翡 ���� �Լ�
    {
        Vector3 dir;
        if (!TryGetFireDir(out dir))
            return;

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

    // 발사 방향(단위 벡터)을 구한다.
    // isRandomPlace 무기는 스캐너의 랜덤 방향을, 그 외에는 가장 가까운 적을 향한 방향을 쓴다.
    // 적이 없으면 마지막 방향을 쓰고, 마지막 방향도 없으면(게임 시작 직후) 발사하지 않는다.
    bool TryGetFireDir(out Vector3 dir)
    {
        if (data.isRandomPlace)
        {
            dir = player.scanner.randomTarget.normalized; // randomTarget은 위치가 아니라 방향 벡터
        }
        else if (player.scanner.nearestTarget != null)
        {
            dir = CalcuDistance(player.scanner.nearestTarget.position);
        }
        else
        {
            dir = lastDir;
        }

        if (dir == Vector3.zero)
            return false;

        lastDir = dir;
        return true;
    }

    Vector3 CalcuDistance(Vector3 dest) // 목표 위치를 향한 방향(단위 벡터)
    {
        Vector3 dir = dest - transform.position;
        dir.z = 0;
        return dir.normalized;
    }

    public void LevelUp(float damage, int level) //���Ÿ� �߻�ü�� �ƴѰ��� �߰����� �Լ� ���� �ʿ�(spawn --- �Լ��� �����ʿ�)
    {
        baseStat.Damage = damage * Character.Damage;

        switch (data.LevelUpStatTypes[level])
        {
            case ItemData.SubStatType.AttackSpeed:
                baseStat.AttackSpeed += (baseStat.AttackSpeed * data.subStat[level]);
                break;
            case ItemData.SubStatType.AreaRadius:
                baseStat.AreaRadius += (baseStat.AreaRadius * data.subStat[level]);
                break;
            case ItemData.SubStatType.AreaDuration:
                baseStat.Duration += (baseStat.Duration * data.subStat[level]);
                break;
            case ItemData.SubStatType.ProjectileNum:
                baseStat.ProjectileNum += data.subStat[level];
                break;
            case ItemData.SubStatType.ProjectileSpeed:
                baseStat.ProjectileSpeed += (baseStat.ProjectileSpeed * data.subStat[level]);
                break;
        }

        if(isMaxLevel == false && level+1 == data.damages.Length)
            isMaxLevel = true;

        RefreshStat();
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
            Vector3 dir = player.scanner.randomTarget.normalized; // randomTarget은 이미 방향 벡터
            BackDotObject.position = transform.position;
            BackDotObject.rotation = Quaternion.identity;
            BackDotObject.GetComponent<EvolvedBackDot>().init(stat, dir, data.isKnockback);
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
            for (int i = 0; i < stat.ProjectileNum; i++) 
            {
                Vector3 dir;
                if (!TryGetFireDir(out dir))
                {
                    yield return null; // 쏠 방향이 없으면 다음 프레임에 다시 확인
                    continue;
                }

                bullet = GameManager.Instance.pool.Get(prefabId).transform;
                bullet.position = transform.position;
                // 총알류(1, 4)는 날아가는 방향으로 회전, 나머지는 회전하지 않음
                bullet.rotation = (data.itemId == 1 || data.itemId == 4) ? Quaternion.FromToRotation(Vector3.up, dir) : Quaternion.identity;
                switch (data.itemId)
                {
                    case 1:
                        bullet.GetComponent<Bullet>().init(stat, dir, data.isKnockback);
                        break;
                    case 2:
                        bullet.GetComponent<Projectile>().init(stat, stat.ProjectileSpeed, dir, data.isKnockback, isEvolved);
                        break;
                    case 4: 
                        bullet.GetComponent<Bullet>().init(stat, dir, data.isKnockback);
                        break;
                    case 5:
                        bullet.GetComponent<Projectile>().init(stat, stat.ProjectileSpeed, dir, data.isKnockback, isEvolved);
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

        // evolveData가 비어 있거나 자기 자신을 가리키면 진화 시 스탯이 Lv1로 초기화되므로 진화하지 않는다.
        if (data.evolveData == null || data.evolveData == data)
        {
            Debug.LogWarning($"[{data.itemName}] evolveData가 설정되지 않아 진화를 건너뜁니다.");
            return;
        }

        isEvolved = true;
        ApplyEvolve();
    }

    void ApplyEvolve()
    {
        data = data.evolveData;
        baseStat.init(data); // 진화 데이터의 스탯은 기본값(절대값)으로 적용되고, 패시브와 상한은 RefreshStat에서 다시 적용된다.
        RefreshStat();
        if (id == 3)
            GetComponentInChildren<SlowArea>(true)?.init(stat, true);   // 이미 생성된 영역에 진화 상태 전달
        if (id == 7)
            GetComponentInChildren<Laser>(true)?.init(stat, data.isKnockback, true); // 이미 생성된 레이저에 진화 상태 전달
        if (id == 8)
            SpawnFront();
        if (id == 9)
            StartCoroutine(ShootBackDot());

    }
}
