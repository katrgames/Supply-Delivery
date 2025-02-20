using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{

    public Quest CurrentQuest { get; set; }

    public TMP_Text questDescriptionText;
    public bool IsActiveToQuest { get; set; } = true;

    public TMP_Text scoreReplyText;
    public TMP_Text dialogText;
    private void Start()
    {
        questDescriptionText.text = null;
        scoreReplyText.text = null;
        dialogText.text = null;
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

                GameManager.instance.RemoveSpawnedItem(item);
                // Remove delivered item
                //Destroy(item.gameObject);

                   // Clear quest
                   CurrentQuest = null;
                   IsActiveToQuest = true;
                questDescriptionText.text = null;
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
                dialogText.text = solution.scoreReply;
                scoreReplyText.text = solution.dialogReply;
                    Debug.Log("Correct item! Reply: " + solution.scoreReply);
                    return solution.score; // Return the score from the solution
                }
            }
        // If no match, use the bad solution response
        Debug.Log("Wrong item! Reply: " + CurrentQuest.badSolution.scoreReply);
        dialogText.text = CurrentQuest.badSolution.scoreReply;
        scoreReplyText.text = CurrentQuest.badSolution.dialogReply;
        return 1; // Default bad solution score (adjust if needed)
    }

}
