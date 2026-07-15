using System.Collections;
using System.Threading;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public float damage;
    private float durationTime = 0f;
    private float durationTimer = 0f;
    public bool isEvolved;
    public bool isKnockBack;

    public void init(float damage, float count, bool isKnockBack, bool isEvolved)
    {
        this.damage = damage;
        this.durationTime = count;
        this.isKnockBack = isKnockBack;
        this.isEvolved = isEvolved;
    }
    private void FixedUpdate()
    {
        durationTimer += Time.fixedDeltaTime;
        if (gameObject.CompareTag("ProjectileDot") && isEvolved)
        {
            StartCoroutine(increaseSizeRoutine());
        }
        if (durationTimer > durationTime)
        {
            durationTimer = 0f;
            gameObject.SetActive(false);
        }
    }

    IEnumerator increaseSizeRoutine()
    {
        float increase = 0.1f;
        while (true)
        {
            gameObject.transform.localScale += Vector3.one * increase * Time.deltaTime;
            yield return null;
        }
    }
}
