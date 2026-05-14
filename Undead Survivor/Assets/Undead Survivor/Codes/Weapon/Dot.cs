using System.Threading;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public float damage;
    private float durationTime = 0f;
    private float durationTimer = 0f;
    private float Timer = 0f;


    public void init(float damage, float count)
    {
        this.damage = damage;
        this.durationTime = count;
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
