using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] float spawnY = 7f;

    float spawnInterval = 1.5f;
    float fallSpeed = 4f;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        int lane = Random.Range(0, PlayerController.LanePositions.Length);
        float x = PlayerController.LanePositions[lane];
        var type = (ObstacleType)Random.Range(0, 2);
        GameObject obj = Instantiate(obstaclePrefab, new Vector3(x, spawnY, 0f), Quaternion.identity);
        obj.GetComponent<Obstacle>().Init(fallSpeed, type);
    }

    public void SetDifficulty(float interval, float speed)
    {
        spawnInterval = interval;
        fallSpeed = speed;
    }

    public void StopSpawning()
    {
        enabled = false;
    }

    public void ResetSpawner()
    {
        enabled = true;
        spawnInterval = 1.5f;
        fallSpeed = 4f;
        timer = 0f;

        foreach (var obs in FindObjectsByType<Obstacle>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            Destroy(obs.gameObject);
    }
}
