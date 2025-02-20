using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{

    public Quest CurrentQuest { get; set; }

    public TMP_Text questDescriptionText;
    public bool IsActiveToQuest { get; set; } = true;

    private void Start()
    {
        IsActiveToQuest = true;
    }

    public void AcceptQuest(Quest acceptedQuest)
    {
        CurrentQuest = acceptedQuest;
        UpdateQuestDescription(acceptedQuest);
        IsActiveToQuest = false;

        for (int i = 0; i < acceptedQuest.solutions.Count; i++)
        {

            for (int j = 0; j < GameManager.instance.itemsList.Count; j++)
            {
                Debug.Log("Matching type: " + acceptedQuest.solutions[i].item + " with " + GameManager.instance.itemsList[j].type);
                if (acceptedQuest.solutions[i].item == GameManager.instance.itemsList[j].type)
                {
                    Debug.Log("Spawn item: " + GameManager.instance.itemsList[j]);
                    GameManager.instance.SpawnSolution(GameManager.instance.itemsList[j]);
                }
                
            }
            
        }

    }
    
    public void UpdateQuestDescription(Quest quest)
    {
        questDescriptionText.text = quest != null ? quest.questDescription : "No current quest.";
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

                   // Remove delivered item
                   Destroy(item.gameObject);

                   // Clear quest
                   CurrentQuest = null;
                IsActiveToQuest = true;
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
                    Debug.Log("Correct item! Reply: " + solution.scoreReply);
                    return solution.score; // Return the score from the solution
                }
            }
        // If no match, use the bad solution response
        Debug.Log("Wrong item! Reply: " + CurrentQuest.badSolution.scoreReply);
        return 1; // Default bad solution score (adjust if needed)
    }
}
