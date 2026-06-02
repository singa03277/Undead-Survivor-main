using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float damage;
    private bool hit;
    private float accelerate;
    private Vector3 dir;
    private bool isKnockBack;
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

    public void init(float damage,float acccelerate ,Vector3 dir, bool isKnockBack)
    {
        this.damage = damage;
        this.accelerate = accelerate;
        this.dir = dir;
        this.isKnockBack = isKnockBack;
        hit = false;
        rigid.linearVelocity = dir * 10f;
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

        gameObject.SetActive(false);
    }
}
