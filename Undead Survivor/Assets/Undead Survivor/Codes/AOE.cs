using UnityEngine;

public class AOE : MonoBehaviour
{
    private Vector3 target;
    private bool isArrived = false;
    private float BombTimer = 2f;
    private float Timer = 0f;
    private float Radius = 3f;
    public float damage;
    Rigidbody projectileRB;
    void Awake()
    {
        projectileRB = GetComponent<Rigidbody>();
    }
    public void init(float damage, float degree, float power, Vector3 target)
    {
        this.target = target;
        this.damage = damage;
        float radianAngle = degree * Mathf.Deg2Rad;
        projectileRB.AddForce((new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle), 0) * power), ForceMode.Impulse);
    }
    void FixedUpdate()
    {
        if (this.transform.position == target)
            isArrived = true;

    }
}
