using UnityEngine;

public class RotationWeapon : MonoBehaviour
{
    public float damage;
    public void init(float damage)
    {
        this.damage = damage;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        Enemy hitEnemy = collision.GetComponent<Enemy>();
        hitEnemy.TakeDamage(damage, gameObject.tag);
    }
}
