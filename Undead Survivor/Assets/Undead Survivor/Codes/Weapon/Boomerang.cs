using System.Collections;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    private bool hit = false;
    private float accelerate;
    private Vector3 dir;
    private bool isKnockBack;
    private bool isEvolved;
    [SerializeField] float launchAngle = 15f; // 발사할 때 정면에서 좌우로 틀어지는 최대 각도 (도)
    WeaponStat stat;
    Rigidbody2D rigid;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (hit == true)
        {
            Vector2 decrease = dir * accelerate * Time.deltaTime;
            rigid.linearVelocity -= decrease * 10;

            // 되돌아오는 중(진행 방향이 발사 방향과 반대)에는 현재 투사체 속도를 넘지 않게 한다.
            // (기존에는 감속이 계속 걸려 돌아올수록 끝없이 빨라졌다.)
            if (Vector2.Dot(rigid.linearVelocity, dir) < 0 && rigid.linearVelocity.magnitude > stat.ProjectileSpeed)
                rigid.linearVelocity = rigid.linearVelocity.normalized * stat.ProjectileSpeed;
        }
    }

    public void init(WeaponStat stat,Vector3 dir, bool isKnockBack,bool isEvolved)
    {
        this.stat = stat;
        this.accelerate = 3f;
        // 완전 정면이 아니라 좌우로 살짝 틀어진 방향으로 발사한다.
        this.dir = Quaternion.Euler(0, 0, Random.Range(-launchAngle, launchAngle)) * dir;
        this.isKnockBack = isKnockBack; // 넉백 값이 저장되지 않아 항상 false였음
        this.isEvolved = isEvolved;
        hit = false;
        transform.localScale = Vector3.one; // 풀에서 재사용될 때 진화 크기가 남지 않도록 초기화
        rigid.linearVelocity = this.dir * stat.ProjectileSpeed;
        if (isEvolved) 
        {
            StartCoroutine(increaseSizeRoutine());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (hit == false)
                hit = true;
            Enemy hitEnemy = collision.GetComponent<Enemy>();
            hitEnemy.TakeDamage(stat.Damage, gameObject.tag, isKnockBack);
        }
  
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        gameObject.transform.localScale = Vector3.one;
        gameObject.SetActive(false);
        
    }

    IEnumerator increaseSizeRoutine()
    {
        float increase = 0.5f;
        while (true)
        {
            gameObject.transform.localScale += Vector3.one * increase * Time.deltaTime;
            yield return null;
        }
    }
}
