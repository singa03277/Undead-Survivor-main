using UnityEngine;

public class FrontAttack : MonoBehaviour
{
    private float damage;
    private float AttackTime = 1f;
    private float AttackTimer = 0f;
    private float RotateTime = 0.333f;
    private float RotateTimer = 0f;
    private bool IsAttack = false;
    private bool isRight = false;
    private Player play;
    private Vector2 PlayerDir;
    private Vector3 Offset;
    private Vector3 StartAngle;
    public void init(float damage, bool isRight)
    {
        this.damage = damage;
        this.isRight = isRight;
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


    void AttackDir()
    {
        float angle = Mathf.Atan2(PlayerDir.y, PlayerDir.x) * Mathf.Rad2Deg;
        float startOffset = -30f;

        float weaponAngle = isRight ? angle + startOffset : angle - startOffset;

        Offset = Quaternion.Euler(0, 0, weaponAngle) * Vector3.right * 3f;
        transform.position = play.transform.position + Offset;
    }
}

//돌아가는 각도 조절하기