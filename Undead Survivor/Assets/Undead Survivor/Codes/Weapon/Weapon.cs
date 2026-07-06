using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour //무기 각각에 들어가는 스크립트(무기의 id에 따라서 다른 작동을 보여준다)
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
    private bool isEvolved = false; //진화 무기에 사용변수 - 스크립트 오브젝트를 무기 클래스에 넣어서 비교를 해서 사용예정
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
                    Debug.Log(timer);
                    timer = 0;
                    FireRange("Dot");
                    Debug.Log("발사");
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
        name = "Weapon " + data.itemId; //이름 설정
        transform.parent = player.transform; //부모 오브젝트 설정
        //플레이어 안에서 위치를 0, 0, 0으로 맞추기 때문에 LocalPostion 사용
        transform.localPosition = Vector3.zero;

        //Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;
        isRandom = data.isRandomPlace;


        for (int i = 0; i < GameManager.Instance.pool.prefabs.Length; i++)
        {
            //프리팹 아이디는 풀링 매니저의 변수에서 찾아서 초기화

            if (data.projectile == GameManager.Instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        //무기 id에 맞게 무기 속성을 설정
        switch (id)
        {
            case 0:
                speed = 150 * Character.WeaponSpeed;   //마이너스 = 시계방향
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
            default:
                break;
        }
        Hand hand = player.hands[(int)data.itemType]; //아이템 타입에 맞는 핸드를 아이템을 hand에 연결
        hand.spriter.sprite = data.hand; //추가해놨던 hand sprite를 적용
        hand.gameObject.SetActive(true);

        //Weapon이 새롭게 추가되면 ApplyGear로 새롭게 추가된 무기에 Gear 레벨을 적용
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); //플레이어에게 broadcast해주도록 부탁

    }

    void FireRange(string name) // 원거리 발사에 대한 함수
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

    public void LevelUp(float damage, int count) //원거리 발사체가 아닌것은 추가적인 함수 적용 필요(spawn --- 함수들 수정필요)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0)
            Batch();

        //Weapon이 레벨업하면 ApplyGear로 레벨업한 무기에 Gear 레벨을 적용
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); //플레이어에게 broadcast해주도록 부탁
        //플레이어가 가지고 있는 모든 Gear에 한해서 ApplyGear가 실행
    }

    void Batch()// 생성된 무기를 배치하는 함수
    {
        for (int i = 0; i < count; i++)
        {

            Transform bullet;
            
            // 기존 오브젝트를 먼저 활용하고 모자란 것은 풀링에서 가져오기
            if (i < transform.childCount) // 자식을 가지고 있으면 새로 꺼내지 않고
            {
                bullet = transform.GetChild(i);  //기존의 자식들을 가져다 쓴다.
            }
            else
            {
                bullet = GameManager.Instance.pool.Get(prefabId).transform;
                bullet.parent = transform;  //새로 가져오는 것들만 parent를 설정해주면 된다. 
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * i / count; //i번째 무기의 회전 각도를 계산
            bullet.Rotate(rotVec);                              //rotVec만큼 회전
             
            //이동 방향이 Space.self가 아니라 World인 이유는? 이미 회전 후 위쪽 방향으로 1.5만큼 이동시키는 것으로 했으므로 이동 방향은 월드를 기준으로 설정
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

    IEnumerator SequenceShot() 
    {
        while(SequenceCount++ < count)
        {
            FireRange("SequenceBullet");
            yield return new WaitForSeconds(0.1f);
        }
    }

    void EvolveAttack(int prefabId) //아이디 스위치해서 선택할예정
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

// 진화 데이터를 어떻게 얻어 오냐? -> evolve데이터를 아이템 데이터에 넣는다. 그리고 진화를 그것을 받아와서 스탯 반영하기