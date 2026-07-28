using System.Collections;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float damage;
    private bool hit = false;
    private float accelerate;
    private Vector3 dir;
    private bool isKnockBack;
    Rigidbody2D rigid;
    private bool isEvolved;

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

    public void init(float damage,float count ,Vector3 dir, bool isKnockBack,bool isEvolved)
    {
        this.damage = damage;
        this.accelerate = count;
        this.dir = dir;
        hit = false;
        rigid.linearVelocity = dir * 10f;
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
            hitEnemy.TakeDamage(damage, gameObject.tag, isKnockBack);
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
