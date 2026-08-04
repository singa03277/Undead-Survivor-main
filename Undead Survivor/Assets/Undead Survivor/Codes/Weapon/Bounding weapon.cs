using UnityEditor.ShaderGraph;
using UnityEngine;

public class Boundingweapon : MonoBehaviour
{
    private float bounceCount;
    private bool isKnockBack;
    private bool isEvolved = false;

    Rigidbody2D rb;
    WeaponStat stat;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void init(WeaponStat stat ,Vector3 dir, bool isKnockBack, bool isEvolved)
    {
        this.stat = stat;
        this.bounceCount= stat.count;
        this.isKnockBack= isKnockBack;
        this.isEvolved = isEvolved;
        rb.linearVelocity = dir * stat.ProjectileSpeed;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Screen"))
        {
            Vector2 diff = (Vector2)transform.position - collision.ClosestPoint(transform.position);
            Vector2 normal;
            Vector2 Refdir = rb.linearVelocity.normalized;
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                normal = diff.x > 0 ? Vector2.right : Vector2.left;
            }
            else
            {
                normal = diff.y > 0 ? Vector2.up : Vector2.down;
            }

            rb.linearVelocity = Vector2.Reflect(Refdir, normal).normalized * stat.ProjectileSpeed;
            bounceCount--;
        }

        if (collision.CompareTag("Enemy") && isEvolved == false)
        {
            Enemy hitEnemy = collision.GetComponent<Enemy>();
            hitEnemy.TakeDamage(stat.Damage, gameObject.tag, isKnockBack);
        }
        else if(collision.CompareTag("Enemy") && isEvolved == true)
        {
            RaycastHit2D[] Enemys = Physics2D.CircleCastAll(transform.position, 1f, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
            foreach (RaycastHit2D scanEnemy in Enemys)
            {
                Enemy enemy = scanEnemy.collider.GetComponent<Enemy>();
                enemy.TakeDamage(stat.Damage, gameObject.tag, isKnockBack);
            }
        }
    }


    private void FixedUpdate()
    {
        if(bounceCount <= -1)
        {
            gameObject.SetActive(false); 
        }
    }

}
