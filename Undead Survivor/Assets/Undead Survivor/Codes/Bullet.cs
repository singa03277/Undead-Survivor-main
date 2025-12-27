using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;   //������
    public int per;        //����

    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;   //Bullet�� �������� �Ű����� �������� �ʱ�ȭ
        this.per = per;         //Bullet�� ������ �Ű����� �������� �ʱ�ȭ

        if (per >= 0)
        {
            rigid.linearVelocity = dir * 15f; //�ӵ��� 15�� ��� 
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ���Ϳ� ������ �ƴϰų�, ������ ������ ��� return
        if (!collision.CompareTag("Enemy") || per == -100)
            return;

        per--;

        if (per < 0) //per ��ġ��ŭ ���͸� ������ 0���� �۾����� ����
        {
            rigid.linearVelocity = Vector2.zero;//��Ȱ��ȭ ������ ������ ���� �̸� ���� �ӵ� �ʱ�ȭ
            gameObject.SetActive(false); //��Ȱ��ȭ
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
            return;

        gameObject.SetActive(false);
    }
}