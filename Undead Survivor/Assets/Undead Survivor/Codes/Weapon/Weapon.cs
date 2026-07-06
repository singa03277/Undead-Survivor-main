using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour //���� ������ ���� ��ũ��Ʈ(������ id�� ���� �ٸ� �۵��� �����ش�)
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    public bool isKnockBack = false;
    private bool isRandom = false;
    private bool isShoot = false;
    private float SequenceCount;
    private bool isEvolved = false; //��ȭ ���⿡ ��뺯�� - ��ũ��Ʈ ������Ʈ�� ���� Ŭ������ �־ �񱳸� �ؼ� ��뿹��
    public float timer;
    Player player;
    ItemData data;
    void Awake()
    {
        player = GameManager.Instance.player;
    }

    void Update()
    {
        if (!GameManager.Instance.isLive)
            return;

        
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            case 1:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0;
                    FireRange("Bullet");
                }
                break;
            case 2:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0;
                    FireRange("Dot");
                }
                break;
            case 3:
                timer += Time.deltaTime;
                break;
            case 4:
                timer += Time.deltaTime;
                if (isEvolved)
                {
                    if(timer > speed)
                        FireRange("Bullet");
                }
                else if(!isEvolved)
                {
                    if (timer > speed && isShoot == false)
                    {
                        timer = 0;
                        isShoot = true;
                        StartCoroutine("SequenceShot");

                    }
                    if (SequenceCount == count)
                    {
                        SequenceCount = 0;
                        isShoot = false;
                        StopCoroutine("SequenceShot");
                    }
                }

                break;
            case 5:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0;
                    FireRange("Bomb");
                }
                break;
            case 6:
                timer += Time.deltaTime;

                if (timer > speed)
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
                if(timer > speed)
                {
                    timer = 0;
                    SpawnBackDot();
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
        //�÷��̾� �ȿ��� ��ġ�� 0, 0, 0���� ���߱� ������ LocalPostion ���
        transform.localPosition = Vector3.zero;

        //Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;
        isRandom = data.isRandomPlace;


        for (int i = 0; i < GameManager.Instance.pool.prefabs.Length; i++)
        {
            //������ ���̵�� Ǯ�� �Ŵ����� �������� ã�Ƽ� �ʱ�ȭ

            if (data.projectile == GameManager.Instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        //���� id�� �°� ���� �Ӽ��� ����
        switch (id)
        {
            case 0:
                speed = 150 * Character.WeaponSpeed;   //���̳ʽ� = �ð����
                Batch();
                break;
            case 1:
                speed = 0.3f * Character.WeaponRate;    
                break;
            case 2:
                speed = 10 * Character.WeaponRate;
                break;
            case 3:
                speed = 10 * Character.WeaponRate;
                SpawnArea();
                break;
            case 4:
                speed = 5f * Character.WeaponRate;
                break;
            case 5:
                speed = 0.3f * Character.WeaponRate;
                break;
            case 6:
                speed = 0.3f * Character.WeaponRate;
                break;
            case 7:
                speed = 10 * Character.WeaponRate;
                SpawnLaser();
                break;
            case 8:
                speed = 10 * Character.WeaponRate;
                SpawnFront();
                break;
            case 9:
                speed = 5f * Character.WeaponRate;
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
        if (isRandom)
        {
            dir = CalcuDistance(player.scanner.randomTarget);
        }
        else
        {
            dir = CalcuDistance(player.scanner.nearestTarget.position);
        }
            
        if (dir == Vector3.zero)
            return;
        Transform Range = GameManager.Instance.pool.Get(prefabId).transform;
        Range.position = transform.position;
        Range.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        switch (name)
        {
            case "Bullet":
                Range.GetComponent<Bullet>().init(damage, count, dir, data.isKnockback);
                break;
            case "Boomerang":
                Range.GetComponent<Boomerang>().init(damage, 3f, dir, data.isKnockback);
                break;
            case "Bomb":
                Range.GetComponent<Projectile>().init(damage, count, Random.Range(30f, 80f), Random.Range(8f, 11f), dir, data.isKnockback);    
                break;
            case "Bounding":
                Range.GetComponent<Boundingweapon>().init(damage, count, dir, data.isKnockback, false);
                break;
            case "Dot":
                Range.GetComponent<Projectile>().init(damage,count, Random.Range(30f, 80f), Random.Range(8f, 11f), dir, data.isKnockback);
                break;
            case "SequenceBullet":
                Range.GetComponent<Bullet>().init(damage, 0, dir, data.isKnockback);
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

    public void LevelUp(float damage, int count) //���Ÿ� �߻�ü�� �ƴѰ��� �߰����� �Լ� ���� �ʿ�(spawn --- �Լ��� �����ʿ�)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0)
            Batch();

        //Weapon�� �������ϸ� ApplyGear�� �������� ���⿡ Gear ������ ����
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); //�÷��̾�� broadcast���ֵ��� ��Ź
        //�÷��̾ ������ �ִ� ��� Gear�� ���ؼ� ApplyGear�� ����
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
            bullet.GetComponent<RotationWeapon>().init(damage,data.isKnockback);
        }
    }
    void SpawnLaser()
    {
        Transform Laser;
        Laser = GameManager.Instance.pool.Get(prefabId).transform;
        Laser.parent = transform;

        Laser.localPosition = Vector3.zero;
        Laser.localRotation = Quaternion.identity;

        Laser.GetComponent<Laser>().init(damage,count,isEvolved);
    }

    void SpawnFront()
    {
        Transform Front;
        Front = GameManager.Instance.pool.Get(prefabId).transform;
        Front.parent = transform;

        Front.localPosition = Vector3.zero;
        Front.localRotation = Quaternion.identity;

        if (isEvolved) 
        {
            Front.GetComponent<FrontAttack>().init(damage, true);
            return;
        }
        else
            Front.GetComponent<FrontAttack>().init(damage, false);
    }
    
    void SpawnArea()
    {
        Transform Area;

        Area = GameManager.Instance.pool.Get(prefabId).transform;
        Area.parent = transform;

        Area.localPosition = Vector3.zero;
        Area.localRotation = Quaternion.identity;
        Area.GetComponent<SlowArea>().init(damage, count,isEvolved);
    }

    void SpawnBackDot()
    {
        Transform Dot = GameManager.Instance.pool.Get(prefabId).transform;

        Dot.position = transform.position;
        Dot.rotation = Quaternion.identity;
        Dot.GetComponent<Dot>().init(damage, count, data.isKnockback);
    }

    IEnumerator SequenceShot() 
    {
        while(SequenceCount++ < count)
        {
            FireRange("SequenceBullet");
            yield return new WaitForSeconds(0.1f);
        }
    }

    void EvolveAttack(int prefabId) //���̵� ����ġ�ؼ� �����ҿ���
    {
        switch (prefabId)
        {
            case 4:

                StartCoroutine(SequenceShot());
                break;
        default:
                break;
        }
    }

}

// ��ȭ �����͸� ��� ��� ����? -> evolve�����͸� ������ �����Ϳ� �ִ´�. �׸��� ��ȭ�� �װ��� �޾ƿͼ� ���� �ݿ��ϱ�