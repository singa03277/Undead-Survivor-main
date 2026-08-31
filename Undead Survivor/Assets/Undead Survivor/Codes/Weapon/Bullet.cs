using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    WeaponStat stat;       
    Rigidbody2D rigid;
    private float per = 0f;
    private bool isKnockBack;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void init(WeaponStat stat, Vector3 dir, bool isKnockBack)
    {
        this.stat = stat;
        per = 3;
        this.isKnockBack = isKnockBack; 
        rigid.linearVelocity = dir * 15f;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        if (collision.CompareTag("Enemy"))
        {
            Enemy hitEnemy = collision.GetComponent<Enemy>();
            hitEnemy.TakeDamage(stat.Damage, gameObject.tag, isKnockBack);
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