using UnityEngine;

public class SequenceBullet : MonoBehaviour
{
    public float damage;       
    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void init(float damage, Vector3 dir)
    {
        this.damage = damage;
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
            gameObject.SetActive(false);
        }
    }
}
