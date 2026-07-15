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

    public void TakeDamage(float damage,string type, bool isKnockBack)
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
}
