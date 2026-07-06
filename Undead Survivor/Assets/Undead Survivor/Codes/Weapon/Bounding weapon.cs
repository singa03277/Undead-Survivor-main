using UnityEditor.ShaderGraph;
using UnityEngine;

public class Boundingweapon : MonoBehaviour
{
    private float damage;
    private float bounceCount;
    private bool isKnockBack;
    private Rigidbody2D rb;
    private bool isEvolved = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void init(float damage, float boundCount,Vector3 dir, bool isKnockBack, bool isEvolve)
    {
        this.damage= damage;
        this.bounceCount= boundCount;
        this.isKnockBack= isKnockBack;
        rb.linearVelocity = dir * 15f;
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

            rb.linearVelocity = Vector2.Reflect(Refdir, normal).normalized * 15f;
            bounceCount--;
        }

        if (collision.CompareTag("Enemy") && isEvolved == false)
        {
            Enemy hitEnemy = collision.GetComponent<Enemy>();
            hitEnemy.TakeDamage(damage, gameObject.tag, isKnockBack);
        }
        else if(collision.CompareTag("Enemy") && isEvolved == true)
        {
            RaycastHit2D[] Enemys = Physics2D.CircleCastAll(transform.position, 1f, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
            foreach (RaycastHit2D scanEnemy in Enemys)
            {
                Enemy enemy = scanEnemy.collider.GetComponent<Enemy>();
                enemy.TakeDamage(damage, gameObject.tag, isKnockBack);
            }
        }
    }


    private void FixedUpdate()
    {
        if(bounceCount <= -1)
        {
            Object.Destroy(gameObject); 
        }
    }

}
