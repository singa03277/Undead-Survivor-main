using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//튕기는 무기 현재 튕기는 로직 및 기타 호환 문제로 뒤로 미룬상태
public class Weapon : MonoBehaviour //무기 각각에 들어가는 스크립트(무기의 id에 따라서 다른 작동을 보여준다)
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    private bool isEvolve = false; //진화 무기 개발 시 사용될 변수

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

                if (timer > speed*10)
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
                    Debug.Log(timer);
                    timer = 0;
                    FireRange("Dot");
                    Debug.Log("발사");
                }
                break;
            case 10:
                timer += Time.deltaTime;
                break;
            case 11:
                timer += Time.deltaTime;
                if( timer > speed)
                {
                    timer = 0;
                    FireRange("SequenceBullet");
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
        
        name = "Weapon " + data.itemId; //이름 설정
        transform.parent = player.transform; //부모 오브젝트 설정
        //플레이어 안에서 위치를 0, 0, 0으로 맞추기 때문에 LocalPostion 사용
        transform.localPosition = Vector3.zero;

        //Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;
        
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
                speed = 10 * Character.WeaponRate;
                break;
            case 10:
                speed = 10 * Character.WeaponRate;
                SpawnArea();
                break;
            case 11:
                speed = 10 * Character.WeaponRate;
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

    void FireRange(string name) //진화 여부에 따라서도 분류할예정
    {
        Vector3 dir = CalcuDistance(player.scanner.nearestTarget.position);
        if (dir == Vector3.zero)
            return;
        Transform Range = GameManager.Instance.pool.Get(prefabId).transform;
        Range.position = transform.position;
        Range.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        switch (name)
        {
            case "Bullet":
                Range.GetComponent<Bullet>().init(damage, count, dir);
                break;
            case "Boomerang":
                Range.GetComponent<Boomerang>().init(damage, 3f, dir);
                break;
            case "Bomb":
                Range.GetComponent<Projectile>().init(damage, count, Random.Range(30f, 80f), Random.Range(8f, 11f), dir);    
                break;
            case "Bounding":
                Range.GetComponent<Boundingweapon>().init(damage, count, dir);
                break;
            case "Dot":
                Range.GetComponent<Projectile>().init(damage,count, Random.Range(30f, 80f), Random.Range(8f, 11f), dir);
                break;
            case "SequenceBullet":
                Range.GetComponent<Bullet>().init(damage, 0, dir);
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

    // Vector3 랜덤으로 나오는 함수도 제작 필요

    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0) //id가 0이면 재배치
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
            bullet.GetComponent<RotationWeapon>().init(damage);
        }
    }
    void SpawnLaser()
    {
        Transform Laser;
        Laser = GameManager.Instance.pool.Get(prefabId).transform;
        Laser.parent = transform;

        Laser.localPosition = Vector3.zero;
        Laser.localRotation = Quaternion.identity;

        Laser.GetComponent<Laser>().init(damage,count);
    }

    void SpawnFront()
    {
        Transform Front;
        Front = GameManager.Instance.pool.Get(prefabId).transform;
        Front.parent = transform;

        Front.localPosition = Vector3.zero;
        Front.localRotation = Quaternion.identity;

        Front.GetComponent<FrontAttack>().init(damage);
    }
    
    void SpawnArea()
    {
        Transform Area;

        Area = GameManager.Instance.pool.Get(prefabId).transform;
        Area.parent = transform;

        Area.localPosition = Vector3.zero;
        Area.localRotation = Quaternion.identity;
        Area.GetComponent<SlowArea>().init(damage, count);
    }
}
