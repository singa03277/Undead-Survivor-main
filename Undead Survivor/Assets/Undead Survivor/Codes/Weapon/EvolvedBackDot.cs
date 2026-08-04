using System.Collections;
using UnityEngine;

public class EvolvedBackDot : MonoBehaviour
{
    public Dot backDotObject;
    Rigidbody2D rigid2D;
    WeaponStat stat;

    float damage;
    float count;
    bool isKnockBack;
    float spawnFreq = 0.5f;
    float flyTime = 3f;
    float flyTimer = 0f;
    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    public void init(WeaponStat stat, Vector3 dir ,bool isKnockBack)
    {
        this.stat = stat;
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
            backDot.init(stat, isKnockBack, false);
            yield return new WaitForSeconds(spawnFreq);
        }

    }
}
