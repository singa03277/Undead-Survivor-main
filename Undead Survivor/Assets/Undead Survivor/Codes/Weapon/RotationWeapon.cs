using UnityEngine;

public class RotationWeapon : MonoBehaviour
{
    public float damage;
    private bool isKnockBack;
    public void init(float damage, bool isKnockBack)
    {
        this.damage = damage;
        this.isKnockBack = isKnockBack;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        Enemy hitEnemy = collision.GetComponent<Enemy>();
        hitEnemy.TakeDamage(damage, gameObject.tag, isKnockBack);
    }
}
