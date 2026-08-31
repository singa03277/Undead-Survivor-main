using UnityEngine;

public class FrontAttack : MonoBehaviour
{
    float AttackTime;
    float AttackTimer = 0f;
    float RotateTime = 0.333f;
    float RotateTimer = 0f;
    bool IsAttack = false;
    bool isRight = false;
    bool isKnockBack;
    WeaponStat stat;
    Player play;
    Vector2 PlayerDir;
    Vector3 Offset;
    Vector3 StartAngle;
    public void init(WeaponStat stat, bool isKnockBack, bool isRight)
    {
        this.stat = stat;
        this.isKnockBack = isKnockBack;
        this.isRight = isRight;
        AttackTime = stat.AttackSpeed;
    }
    private void Awake()
    {
        play = GameManager.Instance.player.GetComponent<Player>();
        transform.rotation=  Quaternion.identity;
    }

    private void FixedUpdate()
    {
        PlayerDir = play.inputVec.normalized;
        
        if (!IsAttack)
        {
            AttackTimer += Time.deltaTime;
            if (AttackTimer > AttackTime) 
            {
                AttackDir();
                AttackTimer = 0f;
                IsAttack = true;
                transform.rotation = Quaternion.identity;
            }
        }
        else
        {
            if (!isRight)
            {
                transform.RotateAround(play.transform.position, Vector3.back, 180 * Time.deltaTime);
                transform.Rotate(Vector3.forward, -180 * Time.deltaTime);
            }
            else
            {
                transform.RotateAround(play.transform.position, Vector3.back, -180 * Time.deltaTime);
                transform.Rotate(Vector3.forward, 180 * Time.deltaTime);
            }

            RotateTimer += Time.deltaTime;
            if (RotateTimer > RotateTime)
            {
                RotateTimer = 0f;
                IsAttack = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        if (collision.CompareTag("Enemy"))
        {
            Enemy hitEnemy = collision.GetComponent<Enemy>();
            hitEnemy.TakeDamage(stat.Damage, gameObject.tag, isKnockBack);
        }
    }

    void AttackDir()
    {
        float angle = Mathf.Atan2(PlayerDir.y, PlayerDir.x) * Mathf.Rad2Deg;
        float startOffset = -30f;

        float weaponAngle = isRight ? angle + startOffset : angle - startOffset;

        Offset = Quaternion.Euler(0, 0, weaponAngle) * Vector3.right * 3f;
        transform.position = play.transform.position + Offset;
    }
}
