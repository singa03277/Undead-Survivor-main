using UnityEngine;

public class FrontAttack : MonoBehaviour
{
    private float damage;
    private float AttackTime = 1f;
    private float AttackTimer = 0f;
    private Player play;
    private Vector2 PlayerDir;
    private Vector3 offset;
    
    public void init(float damage)
    {
        this.damage = damage;
    }
    private void Awake()
    {
        play = GameManager.Instance.GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        PlayerDir = play.inputVec.normalized;
        Attack();
    }

    void Attack()
    {
        Vector3 Dir = AttackDir();
        transform.RotateAround(transform.parent.position, Dir, 60 * Time.deltaTime);
        AttackTimer += Time.deltaTime;
        if (AttackTimer < AttackTime) 
        {
            Debug.Log("Attack finish");
        }
    }

    Vector3 AttackDir()
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
        return offset;
    }
}

