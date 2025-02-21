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
    public TMP_Text titleText;
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public float timeLimit = 120f;
    public GameObject player;
    public float towerQuestRequestTime = 7f;
    public Transform spawnPoint;
    public GameObject endGamePanel;
    public TMP_Text endGameScoreText;


    [Header("Gameplay")]
    public List<Quest> questsList = new List<Quest>();
    public List<Items> itemsList = new List<Items>();
    public Tower[] towers;



    // Private Var
    private float towerAcceptQuestTimer;
    private float currentTime;
    private int gameScore;
    private bool isGameActive;


    private List<Quest> previousQuest = new();
    private List<Items> spawnedItems = new();

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        currentTime = timeLimit;
        gameScore = 0;
        isGameActive = false;
        player = null;
        towerAcceptQuestTimer = towerQuestRequestTime;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player != null)
        {
            player.SetActive(false);
            titleText.gameObject.SetActive(true);
            timeText.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(false);
        }
        endGamePanel.SetActive(false);
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

        if (towerAcceptQuestTimer <= 0)
        {
            towerAcceptQuestTimer = towerQuestRequestTime;
            RollQuest();
        } else
        {
            towerAcceptQuestTimer -= Time.deltaTime;
        }
    }

    private void ToggleGameMenu(bool setMenu)
    {
        playButton.interactable = setMenu;
        playButton.gameObject.SetActive(setMenu);
    }

    public void StartGame()
    {
        isGameActive = true;
        ToggleGameMenu(false);
        RollQuest();
        if (player != null)
        {
            player.SetActive(true); 
        }
        titleText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        endGamePanel.SetActive(false);
    }

    public void RollQuest()
    {

        Quest pickedQuest = null;

        do
        {
            int questIndex = Random.Range(0, questsList.Count);
            pickedQuest = questsList[questIndex];
        }

        while (!CheckQuest(pickedQuest));

        SetTowerQuest(pickedQuest);

    }

    private bool CheckQuest(Quest takenQuest)
    {
        if (previousQuest.Count == 0)
        {
            return true;
        }

        for(int i = 0; i < previousQuest.Count; i++)
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
        if (spawnedItems.Count == 0) return true;

        foreach (var item in spawnedItems)
        {
            if (selectedItem.type == item.type && item.gameObject.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }

    public void SpawnSolution(Items spawnItem)
    {
        /*if (CheckAvailableItems(spawnItem))
        {
            spawnedItems.Add(spawnItem);
            SpawnItemInWorld(spawnItem);
        }*/
        if (!CheckAvailableItems(spawnItem)) return; // Prevent multiple active instances

        // Try to find an inactive item of the same type
        Items existingItem = spawnedItems.Find(item => item.type == spawnItem.type && !item.gameObject.activeInHierarchy);

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

    public void RemoveSpawnedItem(Items itemInstance)
    {
        foreach (var item in spawnedItems)
        {
            if (item == itemInstance)
            {
                item.gameObject.SetActive(false); // Disable instead of removing
                Debug.Log("Deactivated item: " + itemInstance.type);
                return;
            }
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
        foreach (Tower tower in towers)
        {
            tower.scoreReplyText.text = "";
        }
        //show end game score
        endGamePanel.SetActive(true);
        endGameScoreText.text = "Final Score: " + gameScore;
        
        titleText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);

    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
