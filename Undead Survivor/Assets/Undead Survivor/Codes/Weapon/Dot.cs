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
    public void init(WeaponStat stat, bool isKnockBack, bool isEvolved)
    {
        this.stat = stat;
        this.durationTime = stat.Duration;
        this.isKnockBack = isKnockBack;
        this.isEvolved = isEvolved;
        if (isEvolved && gameObject.CompareTag("ProjectileDot"))
        {
            Debug.Log("routine start");            
            StartCoroutine(increaseSizeRoutine());
        }
    }
    private void FixedUpdate()
    {
        durationTimer += Time.deltaTime;
        if (durationTimer > durationTime)
        {
            durationTimer = 0f;
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
