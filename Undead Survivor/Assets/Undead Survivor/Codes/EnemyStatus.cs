using System;
using System.Collections;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{

    [Flags]
    public enum enemyStatus
    {
        none = 0,
        projectileDot = 1 << 0,
        backDot = 1 << 1,
        stun = 1 << 2,
        slow = 1 << 3,
    }

    Enemy enemy;
    Coroutine ProjectileRoutine;
    Coroutine BackDotRoutine;
    enemyStatus currentStatus;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    bool addStatus(enemyStatus status)
    {
        if (hasStatus(status))
        {
            return false;
        }
        currentStatus |= status;
        return true;
    }
    bool removeStatus(enemyStatus status)
    {
        if (!hasStatus(status))
        {
            return false;
        }
        currentStatus &= ~status;
        return true;
    }

    bool hasStatus(enemyStatus status)
    {
        return (currentStatus & status) != 0;
    }

    public void applyStatusRoutine(enemyStatus status)
    {
        if (hasStatus(status))
            return;

        if (hasStatus(enemyStatus.stun) && status == enemyStatus.slow)
            return;

        switch (status) 
        {
            case enemyStatus.projectileDot:
                break;
            
            case enemyStatus.backDot:
                break;

            case enemyStatus.stun:
                break;

            case enemyStatus.slow:
                break;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ProjectileDot"))
        {
            if (!addStatus(enemyStatus.projectileDot))
                return;

            Dot collisionDot = collision.GetComponent<Dot>();
            if(collisionDot.isEvolved)
                ProjectileRoutine = StartCoroutine(evolveDotRoutine(collisionDot));
            else
                ProjectileRoutine = StartCoroutine(dotRoutine(collisionDot));
        }
        else if (collision.CompareTag("BackDot"))
        {
            if (!addStatus(enemyStatus.backDot))
                return;

            Dot collisionDot = collision.GetComponent<Dot>();
            BackDotRoutine = StartCoroutine(dotRoutine(collisionDot));
        }
        else if (collision.CompareTag("SlowArea"))
        {
            if (!addStatus(enemyStatus.slow))
                return;
                
            enemy.calcuspeed = enemy.speed * (collision.GetComponent<SlowArea>().slowPer / 100);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ProjectileDot"))
        {
            if (!removeStatus(enemyStatus.projectileDot))
                return;
            StopCoroutine(ProjectileRoutine);
        }
        
        else if (collision.CompareTag("BackDot"))
        {
            if (!removeStatus(enemyStatus.backDot))
                return;
            StopCoroutine(BackDotRoutine);
        }
        else if (collision.CompareTag("SlowArea"))
        {
            if (!removeStatus(enemyStatus.slow))
                return;
            enemy.calcuspeed = enemy.speed;
        }

    }

    public IEnumerator dotRoutine(Dot dot)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            enemy.TakeDamage(dot.damage, "Dot", dot.isKnockBack);
        }
    }

    public void stun(float stunTime)
    {
        Debug.Log("stun");
        StartCoroutine(stunRoutine(stunTime));
    }

    private IEnumerator stunRoutine(float stunTime)
    {
        float currentSpeed = enemy.calcuspeed;
        enemy.calcuspeed = 0;
        yield return new WaitForSecondsRealtime(stunTime);
        if (hasStatus(enemyStatus.slow))
        {
            enemy.calcuspeed = currentSpeed;
        }
        else
        {
            enemy.calcuspeed = enemy.speed;
        }
    }

    public IEnumerator evolveDotRoutine(Dot dot)
    {
        float damage = dot.damage;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            enemy.TakeDamage(damage++, "Dot", dot.isKnockBack);
        }
    }

}
