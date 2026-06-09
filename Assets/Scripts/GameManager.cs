using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] ObstacleSpawner spawner;
    [SerializeField] PlayerController player;
    [SerializeField] Text scoreText;
    [SerializeField] Text bestText;
    [SerializeField] GameObject gameOverPanel;

    float score;
    float bestScore;
    bool isPlaying;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetFloat("BestScore", 0f);
        StartGame();
    }

    void Update()
    {
        if (!isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Return)) StartGame();
            return;
        }

        score += Time.deltaTime;
        scoreText.text = score.ToString("F1") + "s";

        // 難易度を時間で上昇（10秒ごとに加速）
        float t = score / 10f;
        float interval = Mathf.Max(0.4f, 1.5f - t * 0.15f);
        float speed = Mathf.Min(12f, 4f + t * 0.8f);
        spawner.SetDifficulty(interval, speed);
    }

    public void OnPlayerDead()
    {
        isPlaying = false;
        spawner.StopSpawning();

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetFloat("BestScore", bestScore);
        }

        bestText.text = "Best: " + bestScore.ToString("F1") + "s";
        gameOverPanel.SetActive(true);
    }

    public void StartGame()
    {
        score = 0f;
        isPlaying = true;
        gameOverPanel.SetActive(false);
        player.ResetPlayer();
        spawner.ResetSpawner();
    }
}
