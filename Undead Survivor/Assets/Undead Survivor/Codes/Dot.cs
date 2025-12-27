using UnityEngine;

public class Dot : MonoBehaviour
{
    public float tickrate = 0.5f;
    public float radius = 5f;
    public float damage = 1f;
    private float ticktimer = 0;
    void Start()
    {
            
    }

    void Update()
    {
        ticktimer += Time.deltaTime;
        if(ticktimer > tickrate)
        {
            gameObject.tag = "Dot";
            ticktimer = 0f;

        }
        else
        {
            gameObject.tag = "Untagged";
        }
    }
}
