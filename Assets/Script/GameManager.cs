using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Setup")]
    public Button playButton;
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public float timeLimit = 120f;
    public GameObject player;

    [Header("Gameplay")]
    public List<Quest> questsList = new List<Quest>();
    public List<Items> itemsList = new List<Items>();

    // Private Var
    private float currentTime;
    private int gameScore;
    private bool isGameActive;

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

    public void RollQuest() { }

    public void CheckAvailableSolution() { }

    public void SpawnSolution() { }

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
