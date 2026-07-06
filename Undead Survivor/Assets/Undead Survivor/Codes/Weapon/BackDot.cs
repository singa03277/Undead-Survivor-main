using UnityEngine;

public class BackDot : MonoBehaviour
{
    private bool isKnockBack;
    private float damage;
    private float count;
    public void init(float damage, float count, bool isKnockBack)
    {
        this.damage = damage;
        this.count = count;
        this.isKnockBack = isKnockBack;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        if (collision.CompareTag("Enemy"))
        {
            Enemy hitEnemy = collision.GetComponent<Enemy>();
            hitEnemy.TakeDamage(damage, gameObject.tag, isKnockBack);
        }
    }
}
