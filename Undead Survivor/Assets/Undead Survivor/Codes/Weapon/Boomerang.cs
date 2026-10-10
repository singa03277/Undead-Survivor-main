using System.Collections;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    private bool hit = false;
    private float accelerate;
    private Vector3 dir;
    private bool isKnockBack;
    private bool isEvolved;
    WeaponStat stat;
    Rigidbody2D rigid;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (hit == true)
        {
            Vector2 decrease = dir * accelerate * Time.deltaTime;
            rigid.linearVelocity -= decrease * 10;
        }
    }

    public void init(WeaponStat stat,Vector3 dir, bool isKnockBack,bool isEvolved)
    {
        this.stat = stat;
        this.accelerate = 3f;
        this.dir = dir;
        hit = false;
        rigid.linearVelocity = dir * stat.ProjectileSpeed;
        if (isEvolved) 
        {
            StartCoroutine(increaseSizeRoutine());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (hit == false)
                hit = true;
            Enemy hitEnemy = collision.GetComponent<Enemy>();
            hitEnemy.TakeDamage(stat.Damage, gameObject.tag, isKnockBack);
        }
  
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        gameObject.transform.localScale = Vector3.one;
        gameObject.SetActive(false);
        
    }

    IEnumerator increaseSizeRoutine()
    {
        float increase = 0.5f;
        while (true)
        {
            gameObject.transform.localScale += Vector3.one * increase * Time.deltaTime;
            yield return null;
        }
    }
}
