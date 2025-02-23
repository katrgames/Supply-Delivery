using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public Quest CurrentQuest { get; set; }

    public GameObject questDescPanel;
    public TMP_Text questDescText;

    public bool IsActiveToQuest { get; set; } = true;

    private void Start()
    {
        IsActiveToQuest = true;
        StartCoroutine(HideQuestDescription(0f));
    }

    public void AcceptQuest(Quest acceptedQuest)
    {
        CurrentQuest = acceptedQuest;
        ShowQuestDescription(acceptedQuest);
        IsActiveToQuest = false;

        StartCoroutine(SpawnSolutions(acceptedQuest));
    }

    private IEnumerator SpawnSolutions(Quest acceptedQuest)
    {
        foreach (var solution in acceptedQuest.solutions) // Loop through all quest solutions
        {
            Items itemToSpawn = GameManager.instance.itemsList.Find(item =>
                item.type == solution.item
            );

            if (itemToSpawn != null)
            {
                GameManager.instance.SpawnSolution(itemToSpawn); // Spawn item
                Debug.Log("Spawning item: " + itemToSpawn.type);
            }

            yield return new WaitForSeconds(0.8f); // Wait for seconds before spawning the next item
        }
    }

    public void ShowQuestDescription(Quest quest)
    {
        questDescPanel.SetActive(true);
        questDescText.text = quest.questDescription;
    }

    private IEnumerator HideQuestDescription(float delay)
    {
        yield return new WaitForSeconds(delay);
        questDescText.text = ""; // Clear text after 2.5 seconds
        questDescPanel.SetActive(false);
        IsActiveToQuest = true; // Now set it to true
        GameManager.instance.TowerAcceptQuestTimer =
            GameManager.instance.TowerAcceptQuestTimer > 5
                ? 5
                : GameManager.instance.TowerAcceptQuestTimer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item") && CurrentQuest != null)
        {
            Items item = collision.gameObject.GetComponent<Items>();

            if (item != null)
            {
                int score = GetScoreForItem(item);

                GameManager.instance.AddScore(score);
                Debug.Log("Delivered item: " + item.type + " → Score: " + score);

                //GameManager.instance.QuestFulfilled(CurrentQuest);
                // Start coroutine to hide text after seconds
                StartCoroutine(HideQuestDescription(2f));

                Destroy(item.gameObject);
                GameManager.instance.RemovePreviousItems(item);
            }
        }
    }

    private int GetScoreForItem(Items item)
    {
        ItemsType itemType = item.type;
        // Check if the item is a valid solution
        foreach (Solution solution in CurrentQuest.solutions)
        {
            if (solution.item == itemType)
            {
                questDescText.text = solution.dialogReply;
                GameManager.instance.SetScoreReplyText(solution.scoreReply, solution.score);
                Debug.Log("Correct item! Reply: " + solution.scoreReply);
                return solution.score; // Return the score from the solution
            }
        }
        // If no match, use the bad solution response
        Debug.Log("Wrong item! Reply: " + CurrentQuest.badSolution.scoreReply);
        questDescText.text = CurrentQuest.badSolution.dialogReply;

        GameManager.instance.SetScoreReplyText(CurrentQuest.badSolution.scoreReply, 1);
        return 1; // Default bad solution score (adjust if needed)
    }
}
