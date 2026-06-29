using System;
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
    Coroutine Projectile;
    Coroutine BackDot;

    Rigidbody2D rigid;
    Collider2D coll;    
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;

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

    IEnumerator DotCoroutine(Dot dot)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            TakeDamage(dot.damage, "Dot", dot.isKnockBack); 
        }  
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SlowArea") && isSlow == false)
        {
            isSlow = true;
            calcuspeed = speed * ((100f - collision.GetComponent<SlowArea>().slowPer)/100f);
            
        }
        else if (collision.CompareTag("ProjectileDot"))
        {
            statusAdd(1,collision.GetComponent<Dot>());
        }
        else if (collision.CompareTag("BackDot"))
        {
            statusAdd(2, collision.GetComponent<Dot>());
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SlowArea") && isSlow == true)
        {
            isSlow = false;
            calcuspeed = speed;
        }

        else if (collision.CompareTag("ProjectileDot"))
        {
            statusSub(1);
        }
        else if (collision.CompareTag("BackDot"))
        {
            statusSub(2);
        }
    }
    void statusAdd(int newStatus, Dot dot)
    {
        switch (newStatus)
        {
            case 1:
                if(Projectile == null)
                    Projectile = StartCoroutine(DotCoroutine(dot));
                break;
            case 2:
                if(BackDot == null)
                    BackDot = StartCoroutine(DotCoroutine(dot));
                break;
        }
    }
    void statusSub(int subStatus)
    {
        switch (subStatus)
        {
            case 1:
                if(Projectile != null)
                {
                    StopCoroutine(Projectile);
                    Projectile = null;
                }
                break;
            case 2:
                if(BackDot != null)
                {
                    StopCoroutine(BackDot);
                    BackDot = null;
                }
                break;
        }
    }

}

