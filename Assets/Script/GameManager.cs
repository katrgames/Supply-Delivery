using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Button playButton;
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public float timeLimit = 120f;
    private float currentTime;
    private int gameScore;
    private bool isGameActive;
    public GameObject player;

    void Start()
    {
        playButton.onClick.AddListener(StartGame);
        currentTime = timeLimit;
        gameScore = 0;
        isGameActive = false;
        player = null;
    }

    void Update()
    {
        if (!isGameActive)
        {
            return;
        }

        currentTime -= Time.deltaTime;
        UpdateScoreText();
        UpdateTimeText();

        if (currentTime <= 0)
        {
            EndGame();
        }
    }
    void StartGame()
    {
        isGameActive = true;
        playButton.interactable = false;
        playButton.gameObject.SetActive(false);

    }

    void EndGame()
    {
        isGameActive = false;
        playButton.interactable = true;
        playButton.gameObject.SetActive(true);
        Destroy(player);
    }

    public void AddScore(int score)
    {
        gameScore += score;
        UpdateScoreText();
    }

    void UpdateTimeText()
    {
        timeText.text = "Time:" + currentTime;
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + gameScore;
    }
}
