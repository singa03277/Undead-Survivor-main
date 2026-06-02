using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float calcuspeed;
    public float health;        //ü��
    public float maxHealth;     //�ִ� ü��
    public RuntimeAnimatorController[] animCon;     //������ �ִϸ����͸� �ٲٱ� ���� ��Ʈ�ѷ�
    public Rigidbody2D target; 
    bool isLive;
    private bool isDot = false;
    private bool isSlow = false;

    Rigidbody2D rigid;
    Collider2D coll;    
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;
    Dot DotArea;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }
    
    void FixedUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;

        if (isDot)
        {
            StartCoroutine("DotRoutine");
        }

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) //�׾����� ����
            return;

        Vector2 dirVec = target.position - rigid.position; // ���� = ��ġ ������ ����ȭ (��ġ ���� = Ÿ�� ��ġ - ���� ��ġ)
        Vector2 nextVec = dirVec.normalized * calcuspeed * Time.fixedDeltaTime; 
        rigid.MovePosition(rigid.position + nextVec); 
        rigid.linearVelocity = Vector2.zero; 
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;

        if (!isLive) 
            return;
 
        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable() //Ȱ��ȭ �� �� �� �� ����
    {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true; //�������� �ʱ�ȭ
        // Dead ������Ʈ�� �ٽ� �ʱ�ȭ
        coll.enabled = true;    
        rigid.simulated = true;    
        spriter.sortingOrder = 2;   
        anim.SetBool("Dead", false);  
        health = maxHealth;
    }

    public void Init(SpawnData data) //�ʱ� �Ӽ��� �����ϴ� �Լ� �ۼ�
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];  //�ִϸ��̼� ����
        speed = data.speed;         //�ӵ� ����
        maxHealth = data.health;    //ü�� ����
        health = data.health;
        calcuspeed = speed;
    }

    public void TakeDamage(float damage,string type, bool isKnockBack) //해당 함수로 변경 : 기타 무기와 호환을 위해서 collision -> 함수로 대체 (무기 타입에 따라서 knockback 함수 실행예정)
    {
        health -= damage;
        if (health > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
            if (isKnockBack)
            {
                StartCoroutine(KnockBack());
            }
        }
        else
        {
            coll.enabled = false;      
            rigid.simulated = false;   
            spriter.sortingOrder = 1;  
            anim.SetBool("Dead", true);  
            GameManager.Instance.kill++;
            GameManager.Instance.GetExp();
            if (GameManager.Instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait; // ���� �ϳ��� ���� �������� ������
        Vector3 playerPos = GameManager.Instance.player.transform.position; //Player�� Postion
        Vector3 dirVec = transform.position - playerPos;                    //Player���� Enemy���� ����(=�˹� ����)
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse); //�������� ���̹Ƿ� ForceMode2D.Impulse ����
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }

    IEnumerator DotCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            TakeDamage(DotArea.damage, "Dot",DotArea.isKnockBack); 
        }  
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SlowArea") && isSlow == false)
        {
            isSlow = true;
            calcuspeed = speed * ((100f - collision.GetComponent<SlowArea>().slowPer)/100f);
            
        }
        else if (collision.CompareTag("Dot"))
        {
            isDot = true;
            DotArea = collision.GetComponent<Dot>();
            StartCoroutine("DotCoroutine");
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SlowArea") && isSlow == true)
        {
            isSlow = false;
            calcuspeed = speed;
        }
        else if(collision.CompareTag("Dot")){
            isDot = false;
            StopCoroutine("DotCoroutine");
        }
    }
}

//무기 다 만들고 할일 : 함수 리팩토링(itemdata와 연결해서 조금 더 유연하게 코드를 짠다. - 아이템 무기 유형 등으로 계산해서 바로 가져오기 가능하게 시도), 그리고 데이터에 넉백여부도 확인해서 거기에 맞춰서 넉백 넣기
//0514 재수정 - 게임씬에서 weapon 클래스로 존재하는데 해당 클래스에 id를 통해서 아이템 데이터에 대한 정보를 남김 - 