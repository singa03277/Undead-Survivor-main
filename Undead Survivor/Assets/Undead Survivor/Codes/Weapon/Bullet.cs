using System;
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

    public void init(float damage, int per, Vector3 dir)
    {
        this.damage = damage; 
        this.per = per;         
        rigid.linearVelocity = dir * 15f; 
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        if (collision.CompareTag("Enemy"))
        {
            Enemy hitEnemy = collision.GetComponent<Enemy>();
            hitEnemy.TakeDamage(damage, gameObject.tag);
            per--;
        }
        if (per < 0) 
        {
            rigid.linearVelocity = Vector2.zero; 
            gameObject.SetActive(false); 
        }
    }



    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
            return;
            
        gameObject.SetActive(false);
    }
}