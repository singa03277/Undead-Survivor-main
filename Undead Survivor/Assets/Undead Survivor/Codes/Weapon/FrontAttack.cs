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
    Vector2 lastDir = Vector2.right; // 멈춰 있을 때 공격할 방향(마지막 이동 방향)
    private void Awake()
    {
        play = GameManager.Instance.player.GetComponent<Player>();
        transform.rotation=  Quaternion.identity;
    }

    private void FixedUpdate()
    {
        // 입력이 없으면 방향이 (0,0)이 되어 항상 오른쪽만 공격하던 문제 → 마지막 이동 방향 유지
        if (play.inputVec != Vector2.zero)
            lastDir = play.inputVec.normalized;
        PlayerDir = lastDir;

        // 레벨업으로 줄어든 공격 간격이 반영되도록 매번 stat에서 읽는다.
        AttackTime = stat.AttackSpeed;

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
