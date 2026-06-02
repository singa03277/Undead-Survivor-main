using System.Threading;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public float damage;
    private float durationTime = 0f;
    private float durationTimer = 0f;
    public bool isKnockBack;

    public void init(float damage, float count, bool isKnockBack)
    {
        this.damage = damage;
        this.durationTime = count;
        this.isKnockBack = isKnockBack;
    }
    private void FixedUpdate()
    {
        durationTimer += Time.fixedDeltaTime;
        if (durationTimer > durationTime)
        {
            durationTimer = 0f;
            gameObject.SetActive(false);
        }
    }
}
