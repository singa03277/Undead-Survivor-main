using UnityEngine;

public class SlowArea : MonoBehaviour
{
    float radius;
    float stunTimer = 0f;
    float stunTime;
    public float slowPer;
    bool isEvolved = false;

    WeaponStat stat;
    CircleCollider2D cirCollider;

    private void Awake()
    {
        cirCollider = GetComponent<CircleCollider2D>();
    }

    public void init(WeaponStat stat, bool isEvolved)
    {
        this.stat = stat;
        this.radius = stat.AreaRadius;
        this.slowPer = stat.count;
        this.isEvolved = isEvolved;

        cirCollider.radius = radius;
        stunTime = stat.AttackSpeed;
    }

    private void FixedUpdate()
    {
        stunTime += Time.deltaTime;

        if (isEvolved && (stunTime > stunTimer)) 
        {
            stunTime = 0f;
            RaycastHit2D[] Enemys = Physics2D.CircleCastAll(transform.position, 3f, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
            Debug.Log(Enemys.Length);
            foreach (RaycastHit2D scanEnemy in Enemys)
            {
                EnemyStatus enemy = scanEnemy.collider.GetComponent<EnemyStatus>();
                enemy.stun(2);
            }
        }
    }
}


