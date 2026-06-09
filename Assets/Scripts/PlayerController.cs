using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static readonly float[] LanePositions = { -4f, -2f, 0f, 2f, 4f };

    [SerializeField] float slideSpeed = 15f;

    int currentLane = 2;
    float targetX;
    bool isAlive = true;

    void Start()
    {
        targetX = LanePositions[currentLane];
        transform.position = new Vector3(targetX, transform.position.y, 0f);
    }

    void Update()
    {
        if (!isAlive) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            MoveToLane(currentLane - 1);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            MoveToLane(currentLane + 1);

        float x = Mathf.MoveTowards(transform.position.x, targetX, slideSpeed * Time.deltaTime);
        transform.position = new Vector3(x, transform.position.y, 0f);

        var hit = Physics2D.OverlapBox(transform.position, Vector2.one * 0.75f, 0f);
        if (hit != null && hit.CompareTag("Obstacle"))
        {
            isAlive = false;
            GameManager.Instance.OnPlayerDead();
        }
    }

    void MoveToLane(int lane)
    {
        currentLane = Mathf.Clamp(lane, 0, LanePositions.Length - 1);
        targetX = LanePositions[currentLane];
    }

    public void ResetPlayer()
    {
        isAlive = true;
        currentLane = 2;
        targetX = LanePositions[currentLane];
        transform.position = new Vector3(targetX, transform.position.y, 0f);
    }
}
