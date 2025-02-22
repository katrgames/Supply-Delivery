using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Items nearestItem;

    [Header("Item Interaction")]
    private Items heldItem;
    public Transform holdPoint; // Position where the item will be held
    public Transform dropPoint;
    public Transform grabPoint;
    public float grabRange = 0.2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get input from WASD keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
        FindNearestItem();
        // Move
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, movement * moveSpeed, 0.1f);

        // Flip sprite based on movement direction
        if (movement.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Facing right
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Facing left
        }

        // Set walking animation
        anim.SetBool("Walk", movement.magnitude > 0);
        anim.SetBool("Grab", heldItem != null);
        // Handle grabbing and dropping items
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (heldItem == null) // Grab an item
            {
                if (
                    nearestItem != null
                    && Vector2.Distance(grabPoint.position, nearestItem.transform.position)
                        <= grabRange
                )
                {
                    if (nearestItem.isSpawning)
                        return;
                    heldItem = nearestItem;
                    heldItem.GrabItem(holdPoint); // Call GrabItem from Items.cs
                }
            }
            else // Drop the item
            {
                heldItem.DropItem(dropPoint.position); // Call DropItem from Items.cs
                heldItem = null;
            }
        }
    }

    private void FindNearestItem()
    {
        Items[] items = FindObjectsByType<Items>(FindObjectsSortMode.None);
        float minDistance = Mathf.Infinity;
        nearestItem = null;

        foreach (Items item in items)
        {
            float distance = Vector2.Distance(grabPoint.position, item.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestItem = item;
            }
        }
    }
}
