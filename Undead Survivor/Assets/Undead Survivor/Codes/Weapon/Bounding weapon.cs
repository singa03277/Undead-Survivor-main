using UnityEngine;

public class Boundingweapon : MonoBehaviour
{
    private float damage;
    private float bounceCount;
    private float per;
    private Vector2 RefDir;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Init(float damage, Vector3 dir, float boundCount, float per)
    {
        this.damage= damage;
        this.bounceCount= boundCount;
        this.per= per;
        rb.linearVelocity = dir * 15f;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            RefDir = Vector2.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
        }
        rb.linearVelocity = RefDir * 15f;
    }
}
