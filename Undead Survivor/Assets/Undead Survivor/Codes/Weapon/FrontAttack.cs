using UnityEngine;

public class FrontAttack : MonoBehaviour
{
    private float damage;
    private float AttackTime = 1f;
    private float AttackTimer = 0f;
    private float RotateTime = 0.333f;
    private float RotateTimer = 0f;
    private bool IsAttack = false;
    private Player play;
    private Vector2 PlayerDir;
    private Vector3 Offset;
    private Vector3 StartAngle;
    public void init(float damage)
    {
        this.damage = damage;
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
            transform.RotateAround(play.transform.position, Vector3.back, 180 * Time.deltaTime);
            transform.Rotate(Vector3.forward, -180 * Time.deltaTime);
            RotateTimer += Time.deltaTime;
            if (RotateTimer > RotateTime)
            {
                RotateTimer = 0f;
                IsAttack = false;
                
            }
        }

    }


    void AttackDir()
    {
        if (PlayerDir.x == 0)
        {
            if (PlayerDir.y == 1)
            {
                Offset = Quaternion.Euler(0, 0, 120) * Vector3.right * 3f;
                StartAngle = new Vector3(0, 0, 300);
            }
            else if (PlayerDir.y == -1)
            {
                Offset = Quaternion.Euler(0, 0, -60) * Vector3.right * 3f;
                StartAngle = new Vector3(0, 0, 120);
            }
        }
        else if (PlayerDir.y == 0)
        {
            if (PlayerDir.x == 1)
            {
                Offset = Quaternion.Euler(0, 0, 30) * Vector3.right * 3f;
                StartAngle = new Vector3(0, 0, 210);
            }
            else
            {
                Offset = Quaternion.Euler(0, 0, 210) * Vector3.right * 3f;
                StartAngle = new Vector3(0, 0, 30);
            }
        }
        else
        {
            if (PlayerDir.x > 0)
            {
                if (PlayerDir.y > 0)
                {
                    Offset = Quaternion.Euler(0, 0, 75) * Vector3.right * 3f;
                    StartAngle = new Vector3(0, 0, 225);
                }
                else
                {
                    Offset = Quaternion.Euler(0, 0, 345) * Vector3.right * 3f;
                    StartAngle = new Vector3(0, 0, 165);
                }
            }
            else
            {
                if (PlayerDir.y > 0)
                {
                    Offset = Quaternion.Euler(0, 0, 165) * Vector3.right * 3f;
                    StartAngle = new Vector3(0, 0, 345);
                }
                else
                {
                    Offset = Quaternion.Euler(0, 0, 255) * Vector3.right * 3f;
                    StartAngle = new Vector3(0, 0, 75);
                }
            }
        }
        transform.Rotate(StartAngle);
        transform.position = transform.parent.position + Offset;
    }
}

//돌아가는 각도 조절하기