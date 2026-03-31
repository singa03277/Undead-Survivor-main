using UnityEngine;

public class Boundingweapon : MonoBehaviour
{
    private float damage;
    private float bounceCount;
    private Vector2 dir;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void init(float damage, float boundCount,Vector3 dir)
    {
        this.damage= damage;
        this.bounceCount= boundCount;
        this.dir = dir;
        rb.linearVelocity = dir * 15f;
        
    }

    void OnCollision2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            bounceCount--;
        }
        rb.linearVelocity = dir * 15f;
    }

    

    private void FixedUpdate()
    {
        if(bounceCount <= -1)
        {
            gameObject.SetActive(false);
        }
    }

}
