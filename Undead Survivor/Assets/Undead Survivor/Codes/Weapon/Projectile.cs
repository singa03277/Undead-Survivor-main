using UnityEngine;

public class Projectile : MonoBehaviour
{
    // 포물선을 그리는 무기의 스프라이트에 태그로 Bomb, nonBomb 지정해서 생성하기
    private Vector3 target;
    private bool isArrived = false;
    private float BombTimer = 2f;
    private float Timer = 0f;
    private float Radius;
    private bool isKnockBack;
    private bool isEvolved;
    private bool isSub;
 

    private RaycastHit2D[] Enemys;
    public Dot DotObject;
    public Projectile SubBomb;

    WeaponStat stat;
    Rigidbody projectileRB;
    
    private void Awake()
    {
        projectileRB = GetComponent<Rigidbody>();
    }

    public void init(WeaponStat stat,float power, Vector3 target, bool isKnockBack,bool isEvolved ,bool isSub = false)
    {
        Timer = 0f;
        this.stat = stat;
        isArrived = false;
        this.target = target;
        this.isKnockBack = isKnockBack;
        this.isSub = isSub;
        this.isEvolved = isEvolved;
        this.Radius = stat.AreaRadius;

        if (DotObject == null)
            gameObject.tag = "Bomb";
        else
            gameObject.tag = "Dot";

        if (isSub)
            Radius /= 2;

        projectileRB.AddForce(target * power, ForceMode.Impulse);
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
                SpawnEvolveBomb();
                isEvolved = false;
            }
            Timer += Time.fixedDeltaTime;
            if (Timer >= BombTimer)
            {
                Enemys = Physics2D.CircleCastAll(transform.position, Radius, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
                foreach (RaycastHit2D scanEnemy in Enemys)
                {
                    Enemy enemy = scanEnemy.collider.GetComponent<Enemy>();
                    enemy.TakeDamage(stat.Damage, gameObject.tag, isKnockBack);
                }
                gameObject.SetActive(false);
            }
        }
        if(isArrived && CompareTag("Dot"))
        {
            Dot dot = Instantiate(DotObject, transform.position, Quaternion.identity);
            dot.init(stat, isKnockBack, isEvolved);
            gameObject.SetActive(false);
        }   
    }

    void SpawnEvolveBomb()
    {
        for (int i = 0; i < 6; i++)
        {
            float angle = 360f * i / 6;
            Projectile subbomb = GameManager.Instance.pool.Get(11).GetComponent<Projectile>();
            subbomb.transform.position = transform.position;
            Vector3 targetAngle = Quaternion.Euler(0, 0, angle) * Vector3.right;
            subbomb.init(stat, 3f, targetAngle, isKnockBack,true,true);
        }
    }
}    
