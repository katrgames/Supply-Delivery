using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{

    public Quest CurrentQuest { get; set; }

    public TMP_Text questDescriptionText;
    public bool IsActiveToQuest { get; set; } = true;

    public TMP_Text scoreReplyText;
    private void Start()
    {
        questDescriptionText.text = null;
        scoreReplyText.text = null;
        IsActiveToQuest = true;
    }

    public void AcceptQuest(Quest acceptedQuest)
    {
        CurrentQuest = acceptedQuest;
        UpdateQuestDescription(acceptedQuest);
        IsActiveToQuest = false;
        
        //for (int i = 0; i < acceptedQuest.solutions.Count; i++)
        //{

        //    for (int j = 0; j < GameManager.instance.itemsList.Count; j++)
        //    {
        //        Debug.Log("Matching type: " + acceptedQuest.solutions[i].item + " with " + GameManager.instance.itemsList[j].type);
        //        if (acceptedQuest.solutions[i].item == GameManager.instance.itemsList[j].type)
        //        {
        //            Debug.Log("Spawn item: " + GameManager.instance.itemsList[j]);
        //            GameManager.instance.SpawnSolution(GameManager.instance.itemsList[j]);
        //        }

        //    }

        //}
        StartCoroutine(SpawnSolutionsWithDelay(acceptedQuest));
    }

    private IEnumerator DelaySetActive()
    {
        yield return new WaitForSeconds(11f); // Wait for 3 seconds
        IsActiveToQuest = true; // Now set it to true
    }

    private IEnumerator SpawnSolutionsWithDelay(Quest acceptedQuest)
    {
        foreach (var solution in acceptedQuest.solutions) // Loop through all quest solutions
        {
            Items itemToSpawn = GameManager.instance.itemsList.Find(item => item.type == solution.item);

            if (itemToSpawn != null)
            {
                GameManager.instance.SpawnSolution(itemToSpawn); // Spawn item
                Debug.Log("Spawning item: " + itemToSpawn.type);
            }

            yield return new WaitForSeconds(0.8f); // Wait for seconds before spawning the next item
        }
    }

    public void UpdateQuestDescription(Quest quest)
    {
        questDescriptionText.text = quest.questDescription;
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
                StartCoroutine(HideQuestDescription());

                GameManager.instance.RemoveSpawnedItem(item);
                // Remove delivered item
                //Destroy(item.gameObject);

                //CurrentQuest = null;
                StartCoroutine(DelaySetActive());
                //IsActiveToQuest = true;
                //questDescriptionText.text = null;
                 }
 
        }
    }
    private IEnumerator HideQuestDescription()
    {
        yield return new WaitForSeconds(2.5f);
        questDescriptionText.text = ""; // Clear text after 2.5 seconds
    }

    private int GetScoreForItem(Items item)
    {
        ItemsType itemType = item.type;
            // Check if the item is a valid solution
            foreach (Solution solution in CurrentQuest.solutions)
            {
                if (solution.item == itemType)
                {
                questDescriptionText.text = solution.dialogReply;
                scoreReplyText.text = solution.scoreReply;
                    Debug.Log("Correct item! Reply: " + solution.scoreReply);
                    return solution.score; // Return the score from the solution
                }
            }
        // If no match, use the bad solution response
        Debug.Log("Wrong item! Reply: " + CurrentQuest.badSolution.scoreReply);
        questDescriptionText.text = CurrentQuest.badSolution.dialogReply;
        scoreReplyText.text = CurrentQuest.badSolution.scoreReply;
        return 1; // Default bad solution score (adjust if needed)
    }

}
