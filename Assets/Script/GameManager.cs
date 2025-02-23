using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Setup")]
    public Button playButton;
    public GameObject titlePanel;
    public GameObject gamePanel;
    public GameObject scorePanel;
    public TMP_Text scoreReplyText;
    public TMP_Text endGameScoreText;
    public GameObject settingPanel;
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public TMP_Text itemDescText;
    public float timeLimit = 120f;
    public GameObject player;
    public float towerQuestRequestTime = 5f;
    public Transform spawnPoint;

    [Header("Gameplay")]
    public List<Quest> questsList = new List<Quest>();
    public List<Items> itemsList = new List<Items>();
    public Tower[] towers;
    public Color minColor = Color.red; // Score = 1
    public Color maxColor = Color.green; // Score = 10

    // Private Var
    public float TowerAcceptQuestTimer { get; set; }
    private float currentTime;
    private int gameScore;
    private bool isGameActive;

    private List<Quest> previousQuest = new();
    private List<Items> spawnedItems = new();
    private Coroutine scoreReply;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentTime = timeLimit;
        gameScore = 0;
        isGameActive = false;
        scoreReplyText.text = "";

        TowerAcceptQuestTimer = towerQuestRequestTime;

        player.SetActive(false);

        ToggleTitleMenu(true);

        ToggleGameMenu(false);
        ToggleSettingMenu(false);
        ToggleScoreMenu(false);
    }

    private void Update()
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

        if (TowerAcceptQuestTimer <= 0)
        {
            TowerAcceptQuestTimer = towerQuestRequestTime;
            RollQuest();
        }
        else
        {
            TowerAcceptQuestTimer -= Time.deltaTime;
        }
    }

    private void ToggleTitleMenu(bool setMenu)
    {
        titlePanel.SetActive(setMenu);
    }

    private void ToggleSettingMenu(bool setMenu)
    {
        settingPanel.SetActive(setMenu);
    }

    private void ToggleScoreMenu(bool setMenu)
    {
        scorePanel.SetActive(setMenu);
    }

    private void ToggleGameMenu(bool setMenu)
    {
        gamePanel.SetActive(setMenu);
    }

    public void StartGame()
    {
        isGameActive = true;
        ToggleGameMenu(false);
        RollQuest();

        player.SetActive(true);

        ToggleTitleMenu(false);

        ToggleGameMenu(true);
    }

    public void RollQuest()
    {
        Quest pickedQuest;

        do
        {
            int questIndex = Random.Range(0, questsList.Count);
            pickedQuest = questsList[questIndex];
        } while (!CheckQuest(pickedQuest));

        SetTowerQuest(pickedQuest);
    }

    private bool CheckQuest(Quest takenQuest)
    {
        if (previousQuest.Count == 0)
        {
            return true;
        }

        for (int i = 0; i < previousQuest.Count; i++)
        {
            if (takenQuest == previousQuest[i])
            {
                return false;
            }
        }
        if (previousQuest.Count > 5)
        {
            previousQuest.RemoveAt(0);
        }
        previousQuest.Add(takenQuest);
        return true;
    }

    private void SetTowerQuest(Quest assignQuest)
    {
        //for (int i =0; i < towers.Length; i++)
        //{
        //if (towers[i].IsActiveToQuest)
        //{
        //  towers[i].AcceptQuest(assignQuest);
        //break;
        //  }
        //}
        List<Tower> availableTowers = new List<Tower>();

        // Collect all active towers
        foreach (var tower in towers)
        {
            if (tower.IsActiveToQuest) //&& !previousQuest.Contains(tower.CurrentQuest))
            {
                availableTowers.Add(tower);
            }
        }

        // If there are active towers, pick one randomly
        if (availableTowers.Count > 0)
        {
            int randomIndex = Random.Range(0, availableTowers.Count);
            availableTowers[randomIndex].AcceptQuest(assignQuest);
            //Tower selectedTower = availableTowers[Random.Range(0, availableTowers.Count)];
            //selectedTower.AcceptQuest(assignQuest);
        }
    }

    //public void QuestFulfilled(Quest fulfilledQuest)
    //{
    //    previousQuest.Add(fulfilledQuest);

    //    // If more than 1 other quests have been assigned, remove the oldest one
    //    if (previousQuest.Count > 2)
    //    {
    //        previousQuest.RemoveAt(0); // Allows the tower to take a quest again
    //    }
    //}

    public bool CheckAvailableItems(Items selectedItem)
    {
        if (spawnedItems.Count == 0)
            return true;

        foreach (var item in spawnedItems)
        {
            if (selectedItem.type == item.type && item.gameObject.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }

    public void ChangeItemDescText(string descText)
    {
        itemDescText.text = descText;
    }

    public void SetScoreReplyText(string replyText, int score)
    {
        if (scoreReply != null)
        {
            StopCoroutine(scoreReply);
        }
        scoreReply = StartCoroutine(ChangeScoreReplyText(replyText, score));
    }

    public IEnumerator ChangeScoreReplyText(string replyText, int score)
    {
        float normalizedScore = (score - 1) / 9f;
        scoreReplyText.color = Color.Lerp(minColor, maxColor, normalizedScore);
        scoreReplyText.text = replyText + " +" + score.ToString();
        yield return new WaitForSeconds(2f);
        scoreReplyText.text = "";
    }

    public void SpawnSolution(Items spawnItem)
    {
        /*if (CheckAvailableItems(spawnItem))
        {
            spawnedItems.Add(spawnItem);
            SpawnItemInWorld(spawnItem);
        }*/
        if (!CheckAvailableItems(spawnItem))
            return; // Prevent multiple active instances

        // Try to find an inactive item of the same type
        Items existingItem = spawnedItems.Find(item =>
            item.type == spawnItem.type && !item.gameObject.activeInHierarchy
        );

        if (existingItem != null)
        {
            // Reactivate the existing item instead of spawning a new one
            existingItem.transform.position = spawnPoint.position;
            existingItem.gameObject.SetActive(true);
            Debug.Log("Reusing existing item: " + existingItem.type);
        }
        else
        {
            // If no inactive item exists, instantiate a new one
            Items newItem = Instantiate(spawnItem, spawnPoint.position, Quaternion.identity);
            spawnedItems.Add(newItem);
            Debug.Log("Spawning new item: " + spawnItem.type);
        }
    }

    public void RemovePreviousItems(Items item)
    {
        if (spawnedItems.Contains(item))
        {
            spawnedItems.Remove(item);
        }
    }

    public void AddScore(int score)
    {
        gameScore += score;
        UpdateScoreText();
    }

    void UpdateTimeText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60); // Get minutes
        int seconds = Mathf.FloorToInt(currentTime % 60); // Get remaining seconds

        timeText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + gameScore;
    }

    private void EndGame()
    {
        isGameActive = false;
        ToggleGameMenu(false);
        Destroy(player);
        if (player != null)
        {
            player.SetActive(false);
        }
        scoreReplyText.text = "Time's up!";
        //show end game score
        ToggleScoreMenu(true);
        endGameScoreText.text = "Final Score: " + gameScore;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
