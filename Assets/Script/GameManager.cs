using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Setup")]
    public Button playButton;
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public float timeLimit = 120f;
    public GameObject player;
    public float towerQuestRequestTime = 10f;
    public Transform spawnPoint;

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
        for (int i =0; i < towers.Length; i++)
        {
            if (towers[i].IsActiveToQuest)
            {
                towers[i].AcceptQuest(assignQuest);
                break;
            }
        }
    }

    public bool CheckAvailableItems(Items selectedItem) 
    {
        if (spawnedItems.Count == 0) return true;

        foreach (var item in spawnedItems)
        {
            if (selectedItem == item)
            {
                return false;
            }
        }
        return true;
    }

    public void SpawnSolution(Items spawnItem)
    {
        if (CheckAvailableItems(spawnItem))
        {
            spawnedItems.Add(spawnItem);
            SpawnItemInWorld(spawnItem);
        }
    }

    private void SpawnItemInWorld(Items spawnItem)
    {
        Instantiate(spawnItem.gameObject, spawnPoint.position, Quaternion.identity);
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
    private void EndGame()
    {
        isGameActive = false;
        ToggleGameMenu(true);
        Destroy(player);
    }
}
