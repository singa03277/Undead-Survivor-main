using UnityEngine;

public class Dot : MonoBehaviour
{
    public float damage;
    private float durationTime = 3f;
    private float durationTimer = 0f;
    private float Timer = 0f;
    private void FixedUpdate()
    {
        if (gameObject.tag == "ProjectileDot")
        {
            durationTimer += Time.fixedDeltaTime;
            if (durationTimer > durationTime)
            {
                durationTimer = 0f;
                gameObject.SetActive(false);
            }
        }
        else
            return;
    }
}
