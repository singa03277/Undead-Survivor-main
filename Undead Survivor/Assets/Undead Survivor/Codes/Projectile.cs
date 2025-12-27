using UnityEngine;

public class Projectile : MonoBehaviour
{
    // 포물선을 그리는 무기의 스프라이트에 태그로 Bomb, nonBomb 지정해서 생성하기
    private Vector3 target;
    private bool isArrived = false;
    private float BombTimer = 2f;
    private float Timer = 0f;
    private float Radius = 3f;
    public float damage;
    private RaycastHit2D[] Enemys;
    Rigidbody projectileRB;
    
    private void Awake()
    {
        projectileRB = GetComponent<Rigidbody>();
    }

    public void init(float damage, float degree, float power, Vector3 target)
    {
        this.target = target;
        this.damage = damage;
        float radianAngle = degree * Mathf.Deg2Rad;
        projectileRB.AddForce((new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle), 0) * power), ForceMode.Impulse);
    }
    private void FixedUpdate()
    {
        if(this.transform.position == target)
            isArrived = true;
        
        if (gameObject.CompareTag("Bomb") && isArrived) 
        {
            Timer += Time.fixedDeltaTime;
            if(Timer >= BombTimer)
            {
                Enemys = Physics2D.CircleCastAll(transform.position, Radius, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
                foreach (RaycastHit2D scanEnemy in Enemys)
                {
                    Enemy enemy = scanEnemy.collider.GetComponent<Enemy>();
                    enemy.TakeDamage(damage, gameObject.tag);
                }
            }
        }
    }

}    
