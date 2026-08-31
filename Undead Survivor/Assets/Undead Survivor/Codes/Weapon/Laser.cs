using System.Threading;
using UnityEngine;

public class Laser : MonoBehaviour
{
    Vector2 PlayerDir;
    Vector3 offset;
    float WeaponTime;
    float WeaponTimer = 0f;
    float WaitTime;
    float WaitTimer = 0f;
    bool isWeaponTime = false;
    bool isEvolved = false;
    bool isKnockBack;

    SpriteRenderer Sprite;
    CapsuleCollider2D Colli;
    Player play;
    WeaponStat stat;

    void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
        Colli = GetComponent<CapsuleCollider2D>();
        play = GameManager.Instance.player.GetComponent<Player>();
    }

    public void init(WeaponStat stat, bool isKnockBack ,bool isEvolved)
    {
        this.stat = stat;
        this.isEvolved = isEvolved;
        this.isKnockBack = isKnockBack;
        WeaponTime = stat.Duration;
        WaitTime = stat.AttackSpeed;

        isWeaponTime = true;
    }
    private void FixedUpdate()
    {
        if (!isEvolved)
        {
            if (isWeaponTime)
            {
                WeaponTimer += Time.deltaTime;
                if (WeaponTimer > WeaponTime)
                {
                    Sprite.enabled = false;
                    Colli.enabled = false;
                    isWeaponTime = false;
                    WeaponTimer = 0f;
                }
            }
            else
            {
                WaitTimer += Time.deltaTime;
                if (WaitTimer > WaitTime)
                {
                    Sprite.enabled = true;
                    Colli.enabled = true;
                    isWeaponTime = true;
                    WaitTimer = 0f;
                }
                return;
            }
        }

        PlayerDir = play.inputVec.normalized;
        LaserDir();
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

    void LaserDir()
    {
        if (PlayerDir.x == 0)
        {
            if (PlayerDir.y == 1)
            {
                offset = Quaternion.Euler(0, 0, 90) * Vector3.right * 1.5f;
            }
            else if (PlayerDir.y == -1)
            {
                offset = Quaternion.Euler(0, 0, -90) * Vector3.right * 1.5f;
            }
        }
        else if (PlayerDir.y == 0)
        {
            if (PlayerDir.x == 1)
            {
                offset = Quaternion.Euler(0, 0, 0) * Vector3.right * 1.5f;
            }
            else
            {
                offset = Quaternion.Euler(0, 0, 180) * Vector3.right * 1.5f;
            }
        }
        else
        {
            if (PlayerDir.x > 0)
            {
                if (PlayerDir.y > 0)
                {
                    offset = Quaternion.Euler(0, 0, 45) * Vector3.right * 1.5f;
                }
                else
                {
                    offset = Quaternion.Euler(0, 0, 315) * Vector3.right * 1.5f;
                }
            }
            else
            {
                if (PlayerDir.y > 0)
                {
                    offset = Quaternion.Euler(0, 0, 135) * Vector3.right * 1.5f;
                }
                else
                {
                    offset = Quaternion.Euler(0, 0, 225) * Vector3.right * 1.5f;
                }
            }

        }
        transform.position = transform.parent.position + offset;
    }
}