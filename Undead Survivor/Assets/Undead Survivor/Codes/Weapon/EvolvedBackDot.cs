using System.Collections;
using UnityEngine;

public class EvolvedBackDot : MonoBehaviour
{
    public Dot backDotObject;
    private Rigidbody2D rigid2D;

    private float damage;
    private float count;
    private bool isKnockBack;
    private float spawnFreq = 0.5f;
    private float flyTime = 3f;
    private float flyTimer = 0f;
    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    public void init(float damage, float count, Vector3 dir ,bool isKnockBack)
    {
        this.damage = damage;
        this.count = count;
        this.isKnockBack = isKnockBack;
        rigid2D.linearVelocity = dir * 5f;
        StartCoroutine(spawnDotRoutine());
    }

    private void FixedUpdate()
    {
        flyTimer += Time.deltaTime;
        if(flyTimer > flyTime)
        {
            flyTimer = 0f;
            rigid2D.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    IEnumerator spawnDotRoutine()
    {
        while (true)
        {
            Dot backDot = Instantiate(backDotObject, transform.position, Quaternion.identity);
            backDot.init(damage, count, isKnockBack, false);
            yield return new WaitForSeconds(spawnFreq);
        }

    }
}
