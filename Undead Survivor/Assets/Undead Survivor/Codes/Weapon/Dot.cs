using System.Collections;
using System.Threading;
using UnityEngine;

public class Dot : MonoBehaviour
{
   
    private float durationTime = 0f;
    private float durationTimer = 0f;
    public bool isEvolved;
    public bool isKnockBack;
    public WeaponStat stat;
    public bool destroyOnEnd = false; // Instantiate로 만든 장판은 끝나면 Destroy (풀 오브젝트는 비활성화만)
    public void init(WeaponStat stat, bool isKnockBack, bool isEvolved)
    {
        this.stat = stat;
        this.durationTime = stat.Duration;
        this.durationTimer = 0f;
        this.isKnockBack = isKnockBack;
        this.isEvolved = isEvolved;
        if (isEvolved && gameObject.CompareTag("ProjectileDot"))
        {           
            StartCoroutine(increaseSizeRoutine());
        }
    }
    private void FixedUpdate()
    {
        durationTimer += Time.deltaTime;
        if (durationTimer > durationTime)
        {
            durationTimer = 0f;
            if (destroyOnEnd)
                Destroy(gameObject); // 비활성화만 하면 계속 쌓여 메모리가 새던 문제
            else
                gameObject.SetActive(false);
        }
    }

    IEnumerator increaseSizeRoutine()
    {
        float increase = 1.3f;
        while (true)
        {
            gameObject.transform.localScale += Vector3.one * increase * Time.deltaTime;
            yield return null;
        }
    }
}
