using UnityEngine;

public class SlowArea : MonoBehaviour
{
    private float radius;
    private bool isEvolved = false;
    private float stunTimer = 3f;
    private float stunTime = 0f;
    public float slowPer;

    private CircleCollider2D cirCollider;

    private void Awake()
    {
        cirCollider = GetComponent<CircleCollider2D>();
    }

    public void init(float radius, float slowPer, bool isEvolved)
    {
        this.radius = radius;
        this.slowPer = slowPer;
        cirCollider.radius = radius;
        this.isEvolved = isEvolved;
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
                Debug.Log(scanEnemy.collider.name);

                Debug.Log(scanEnemy.collider.GetComponent<Enemy>());
                Debug.Log(scanEnemy.collider.GetComponent<EnemyStatus>());
                EnemyStatus enemy = scanEnemy.collider.GetComponent<EnemyStatus>();
                enemy.stun(2);
            }
        }
    }
}


