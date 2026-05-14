using UnityEngine;

public class Projectile : MonoBehaviour
{
    // 포물선을 그리는 무기의 스프라이트에 태그로 Bomb, nonBomb 지정해서 생성하기
    private Vector3 target;
    private bool isArrived = false;
    private float BombTimer = 2f;
    private float Timer = 0f;
    private float Radius = 1f;
    private float count;
    private string Typename;
    public float damage;
    private RaycastHit2D[] Enemys;
    public Dot DotObject;
    Rigidbody projectileRB;
    
    private void Awake()
    {
        projectileRB = GetComponent<Rigidbody>();
    }

    public void init(float damage,float count ,float degree, float power, Vector3 target)
    {
        isArrived = false;
        Timer = 0f;
        this.target = target;
        this.damage = damage;
        this.count = count;
        projectileRB.AddForce(target * power, ForceMode.Impulse);
        if(DotObject == null)
            gameObject.tag = "Bomb";
        else
            gameObject.tag = "Dot";
    }
    private void FixedUpdate()
    {
        if (!isArrived)
        {
            Timer += Time.fixedDeltaTime;
            if (Timer > 0.5f)
            {
                isArrived = true;
                Timer = 0f;
                projectileRB.useGravity = false;
                projectileRB.linearVelocity = Vector3.zero;
            }
        }
        
        if (isArrived && CompareTag("Bomb")) // 일정시간 초 지난 후 폭발
        {
            if (projectileRB.linearVelocity != Vector3.zero)
                projectileRB.linearVelocity = Vector3.zero;
            Timer += Time.fixedDeltaTime;
            if(Timer >= BombTimer)
            {
                Enemys = Physics2D.CircleCastAll(transform.position, Radius, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
                foreach (RaycastHit2D scanEnemy in Enemys)
                {
                    Enemy enemy = scanEnemy.collider.GetComponent<Enemy>();
                    enemy.TakeDamage(damage, gameObject.tag);
                }
                gameObject.SetActive(false);
            }
        }
        if(isArrived && CompareTag("Dot"))
        {
            Dot dot = Instantiate(DotObject, transform.position, Quaternion.identity);
            dot.init(damage, count);
            gameObject.SetActive(false);
        }   
    }

}    
