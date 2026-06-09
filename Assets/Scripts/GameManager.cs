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
    [SerializeField] GameObject titlePanel;

    float score;
    float bestScore;
    bool isPlaying;
    bool isTitle = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetFloat("BestScore", 0f);
        ShowTitle();
    }

    void Update()
    {
        if (isTitle)
        {
            if (Input.GetKeyDown(KeyCode.Return)) StartGame();
            return;
        }

        if (!isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Return)) StartGame();
            return;
        }

        score += Time.deltaTime;
        scoreText.text = score.ToString("F1") + "s";

        float t = score / 10f;
        float interval = Mathf.Max(0.4f, 1.5f - t * 0.15f);
        float speed = Mathf.Min(12f, 4f + t * 0.8f);
        spawner.SetDifficulty(interval, speed);
    }

    void ShowTitle()
    {
        isTitle = true;
        titlePanel.SetActive(true);
        gameOverPanel.SetActive(false);
        scoreText.text = "";
        bestText.text = "Best: " + bestScore.ToString("F1") + "s";
        spawner.StopSpawning();
        player.ResetPlayer();
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
        isTitle = false;
        score = 0f;
        isPlaying = true;
        titlePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        player.ResetPlayer();
        spawner.ResetSpawner();
    }
}
