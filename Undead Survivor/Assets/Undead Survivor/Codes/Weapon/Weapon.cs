using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour //���� ������ ���� ��ũ��Ʈ(������ id�� ���� �ٸ� �۵��� �����ش�)
{
    public int prefabId;
    public int id;
    public int count = 3;
    private Vector3 lastDir;
    private float SequenceCount;
    private bool isEvolved = false; //��ȭ ���⿡ ��뺯�� - ��ũ��Ʈ ������Ʈ�� ���� Ŭ������ �־ �񱳸� �ؼ� ��뿹��
    public float timer;
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
            case 1:
                timer += Time.deltaTime;

                if (timer > stat.AttackSpeed)
                {
                    timer = 0;
                    FireRange("Bullet");
                }
                break;
            case 2:
                timer += Time.deltaTime;
                if (timer > stat.AttackSpeed)
                {
                    timer = 0;
                    FireRange("Dot");
                }
                break;
            case 3:
                timer += Time.deltaTime;
                break;
            case 4:

                break;
            case 5:
                timer += Time.deltaTime;

                if (timer > stat.AttackSpeed)
                {
                    timer = 0;
                    FireRange("Bomb");
                }
                break;
            case 6:
                timer += Time.deltaTime;

                if (timer > stat.AttackSpeed)
                {
                    timer = 0;
                    FireRange("Bounding");
                }
                break;
            case 7:
                timer += Time.deltaTime;

                break;
            case 8:
                timer += Time.deltaTime;
                break;
            case 9:
                timer += Time.deltaTime;
                if(timer > stat.AttackSpeed)
                {
                    timer = 0;
                    //SpawnBackDot();
                }
                break;
            case 13:
                timer += Time.deltaTime;
                if (timer > stat.AttackSpeed)
                {
                    timer = 0;
                    FireRange("Boomerang");
                }
                break;
            default:
                break;

        }

        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 1);
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
            //������ ���̵�� Ǯ�� �Ŵ����� �������� ã�Ƽ� �ʱ�ȭ

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
            case 1:    
                break;
            case 2:
                break;
            case 3:
                SpawnArea();
                break;
            case 4:
                StartCoroutine(SequenceShot());
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                SpawnLaser();
                break;
            case 8:
                SpawnFront();
                break;
            case 9:
                StartCoroutine(ShootBackDot());
                break;
            case 13:
                break;
            default:
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
            dir = player.scanner.nearestTarget == null ? lastDir : CalcuDistance(player.scanner.nearestTarget.position);
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
            case "Boomerang":
                //Range.GetComponent<Boomerang>().init(damage, 3f, dir, data.isKnockback,isEvolved);
                break;
            case "Bomb":
                //Range.GetComponent<Projectile>().init(damage, count, Random.Range(8f, 11f), dir, data.isKnockback,isEvolved);    
                break;
            case "Bounding":
                //Range.GetComponent<Boundingweapon>().init(damage, count, dir, data.isKnockback, isEvolved);
                break;
            case "Dot":
                //Range.GetComponent<Projectile>().init(damage,count, Random.Range(8f, 11f), dir, data.isKnockback,isEvolved);
                break;
            case "SequenceBullet":
                //Range.GetComponent<Bullet>().init(damage, 0, dir, data.isKnockback);
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
        this.count += count;

        switch (data.LevelUpStatTypes[level])
        {
            case ItemData.SubStatType.AttackSpeed:
                //stat.AttackSpeed += stat.AttackSpeed * data.
                break;
        }

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    

    void Batch()// ������ ���⸦ ��ġ�ϴ� �Լ�
    {
        for (int i = 0; i < count; i++)
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

            Vector3 rotVec = Vector3.forward * 360 * i / count; //i��° ������ ȸ�� ������ ���
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

       // Laser.GetComponent<Laser>().init(damage,count,isEvolved);
    }

    void SpawnFront()
    {
        Transform front;

        front = GameManager.Instance.pool.Get(prefabId).transform;
        front.parent = transform;
        front.localPosition = Vector3.zero;
        front.localRotation = Quaternion.identity;

        //front.GetComponent<FrontAttack>().init(damage, false);
        
        if (isEvolved) 
        {
            //front.GetComponent<FrontAttack>().init(damage, true);
            return;
        }
        //else
            //front.GetComponent<FrontAttack>().init(damage, false);
    }
    
    void SpawnArea()
    {
        Transform Area;

        Area = GameManager.Instance.pool.Get(prefabId).transform;
        Area.parent = transform;

        Area.localPosition = Vector3.zero;
        Area.localRotation = Quaternion.identity;
        //Area.GetComponent<SlowArea>().init(damage, count,isEvolved);
    }

    void SpawnBackDot()
    {
        Transform Dot = GameManager.Instance.pool.Get(prefabId).transform;

        Dot.position = transform.position;
        Dot.rotation = Quaternion.identity;
        //Dot.GetComponent<Dot>().init(damage, count, data.isKnockback,isEvolved);
    }


    IEnumerator SequenceShot() 
    {
        while(true)
        {
            float shotCount = 0;
            if (isEvolved)
            {
                FireRange("SequenceBullet");
                yield return new WaitForSeconds(0.12f);
            }
            else
            {
                while(shotCount++ < count)
                {
                    FireRange("SequenceBullet");
                    yield return new WaitForSeconds(0.25f);
                }
            }
        }
    }

    IEnumerator ShootBackDot()
    {
        while (true)
        {
            Transform BackDotObject = GameManager.Instance.pool.Get(12).transform;
            BackDotObject.position = transform.position;
            BackDotObject.rotation = Quaternion.identity;
            //BackDotObject.GetComponent<EvolvedBackDot>().init(damage,count, CalcuDistance(player.scanner.randomTarget), isKnockBack);
            yield return new WaitForSeconds(2f);
        }
    }
}
