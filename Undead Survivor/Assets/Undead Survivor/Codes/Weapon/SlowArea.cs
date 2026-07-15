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
        if (isEvolved) 
        {
            RaycastHit2D[] Enemys = Physics2D.CircleCastAll(transform.position, 1f, Vector2.zero, 0, LayerMask.GetMask("EnemyStatus"));
            foreach (RaycastHit2D scanEnemy in Enemys)
            {
                EnemyStatus enemy = scanEnemy.collider.GetComponent<EnemyStatus>();
                enemy.stunRoutine(2);
            }
        }
    }
}


