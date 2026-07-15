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
    public float damage;
    private bool isKnockBack;
    private bool isEvolved;
    private bool isSub;

    private RaycastHit2D[] Enemys;
    public Dot DotObject;


    Rigidbody projectileRB;
    
    private void Awake()
    {
        projectileRB = GetComponent<Rigidbody>();
    }

    public void init(float damage,float count, float power, Vector3 target, bool isKnockBack, bool isSub = false)
    {
        isArrived = false;
        Timer = 0f;
        this.target = target;
        this.damage = damage;
        this.count = count;
        this.isKnockBack = isKnockBack;
        this.isSub = isSub;
        if (isSub)
            Radius /= 2;
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
        
        if (isArrived && CompareTag("Bomb"))
        {
            if (projectileRB.linearVelocity != Vector3.zero)
                projectileRB.linearVelocity = Vector3.zero;
            if (isEvolved && !isSub)
            {
                spawnEvolveBomb();
            }
            Timer += Time.fixedDeltaTime;
            if (Timer >= BombTimer)
            {
                Enemys = Physics2D.CircleCastAll(transform.position, Radius, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
                foreach (RaycastHit2D scanEnemy in Enemys)
                {
                    Enemy enemy = scanEnemy.collider.GetComponent<Enemy>();
                    enemy.TakeDamage(damage, gameObject.tag, isKnockBack);
                }
                gameObject.SetActive(false);
            }
        }
        if(isArrived && CompareTag("Dot"))
        {
            Dot dot = Instantiate(DotObject, transform.position, Quaternion.identity);
            dot.init(damage, count, isKnockBack,isEvolved);
            gameObject.SetActive(false);
        }   
    }

    void spawnEvolveBomb()
    {
        for (int i = 0; i < 6; i++)
        {
            float angle = 360f * i / 6;
            Vector3 targetAngle = Quaternion.Euler(0, 0, angle) * Vector3.right;
            Projectile subBomb = GameManager.Instance.pool.Get(11).GetComponent<Projectile>();
            subBomb.transform.rotation = Quaternion.Euler(0,0, angle);
            subBomb.init(damage / 2, count, 3f, targetAngle, isKnockBack,true);
        }
    }
}    
