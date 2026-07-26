using System.Threading;
using UnityEngine;

public class Laser : MonoBehaviour
{

    private Vector2 PlayerDir;
    private SpriteRenderer Sprite;
    private CapsuleCollider2D Colli;
    private Player play;
    private float WeaponTimer = 0f;
    private float WeaponTime = 3f;
    private float WaitTime = 5f;
    private float WaitTimer = 0f;
    private bool isWeaponTime = false;
    private Vector3 offset;
    private bool isEvolved = false;
    private float damage;
    void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
        Colli = GetComponent<CapsuleCollider2D>();
        play = GameManager.Instance.player.GetComponent<Player>();
    }

    public void init(float damage, float WeaponTime, bool isEvolved)
    {
        this.damage = damage;
        this.WeaponTime = WeaponTime;
        isWeaponTime = true;
        this.isEvolved = isEvolved;
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