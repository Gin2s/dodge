using UnityEngine;

public enum ObstacleType { SpikeBall, Knife }

public class Obstacle : MonoBehaviour
{
    public ObstacleType Type { get; private set; }
    float speed;

    static readonly Color[] ballColors = {
        new Color(0.9f, 0.2f, 0.3f),
        new Color(0.8f, 0.1f, 0.5f),
        new Color(0.6f, 0.1f, 0.8f),
    };
    static readonly Color[] knifeColors = {
        new Color(0.6f, 0.7f, 0.8f),
        new Color(0.7f, 0.8f, 0.9f),
        new Color(0.5f, 0.65f, 0.75f),
    };

    public void Init(float baseSpeed, ObstacleType type)
    {
        Type = type;
        var sr = GetComponent<SpriteRenderer>();

        if (type == ObstacleType.SpikeBall)
        {
            speed = baseSpeed * 0.7f;
            sr.color = ballColors[Random.Range(0, ballColors.Length)];
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            speed = baseSpeed * 1.5f;
            sr.color = knifeColors[Random.Range(0, knifeColors.Length)];
            transform.localScale = new Vector3(0.45f, 1.2f, 1f);
        }
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y < -8f)
            Destroy(gameObject);
    }
}
