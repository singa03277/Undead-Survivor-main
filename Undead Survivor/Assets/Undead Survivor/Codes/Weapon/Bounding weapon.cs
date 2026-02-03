using UnityEngine;

public class Boundingweapon : MonoBehaviour
{
    private float damage;
    private float bounceCount;
    private Vector2 RefDir;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, float boundCount,Vector3 dir)
    {
        this.damage= damage;
        this.bounceCount= boundCount;
        rb.linearVelocity = dir * 15f;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            Debug.Log("collision");
            RefDir = Vector2.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
            bounceCount--;
        }
        rb.linearVelocity = RefDir * 15f;
    }
    private void FixedUpdate()
    {
        if(bounceCount <= -1)
        {
            gameObject.SetActive(false);
        }
    }

}
