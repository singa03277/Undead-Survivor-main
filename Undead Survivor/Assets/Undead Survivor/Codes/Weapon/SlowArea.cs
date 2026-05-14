using UnityEngine;

public class SlowArea : MonoBehaviour
{
    private float radius;
    public float slowPer;

    private CircleCollider2D cirCollider;
    public void init(float radius, float slowPer)
    {
        this.radius = radius;
        this.slowPer = slowPer;
        cirCollider = GetComponent<CircleCollider2D>();
        cirCollider.radius = radius;
    }
    private void FixedUpdate()
    {

    }
}


