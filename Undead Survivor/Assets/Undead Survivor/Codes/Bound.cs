using UnityEngine;

public class Bound : MonoBehaviour
{
    void Awake()
    {
        Camera Main = transform.parent.GetComponent<Camera>();
        BoxCollider2D colli = GetComponent<BoxCollider2D>();
        float height = Main.orthographicSize * 2f;
        float width = height * Main.aspect;
        colli.size = new Vector2(width, height);
        colli.offset = Vector2.zero;
    }
}
