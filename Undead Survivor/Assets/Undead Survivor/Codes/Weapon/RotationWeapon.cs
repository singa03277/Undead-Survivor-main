using UnityEngine;

public class RotationWeapon : MonoBehaviour
{
    private bool isKnockBack;
    private WeaponStat stat;
    public void init(WeaponStat stat, bool isKnockBack)
    {
        this.stat = stat;
        this.isKnockBack = isKnockBack;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        Enemy hitEnemy = collision.GetComponent<Enemy>();
        hitEnemy.TakeDamage(stat.Damage, gameObject.tag, isKnockBack);
    }
}
