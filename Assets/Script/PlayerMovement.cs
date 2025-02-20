using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    [Header("Item Interaction")]
    private Items heldItem;
    public Transform holdPoint; // Position where the item will be held
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from WASD keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
        // Handle grabbing and dropping items
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
            {
                GrabItem();
            }
            else
            {
                DropItem();
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput = moveInput.normalized;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, movement * moveSpeed, 0.1f);
    }
    private void GrabItem()
    {
        if (heldItem == null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f); // Detect nearby items
            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag("Item"))
                {
                    heldItem = col.GetComponent<Items>();
                    if (heldItem != null)
                    {
                        heldItem.transform.SetParent(holdPoint);
                        heldItem.transform.localPosition = Vector3.zero;
                        heldItem.GetComponent<Rigidbody2D>().simulated = false; // Disable physics
                        Debug.Log("Picked up: " + heldItem.name);
                        return;
                    }
                }
            }
        }
    }

    private void DropItem()
    {
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null);
            heldItem.GetComponent<Rigidbody2D>().simulated = true; // Enable physics
            heldItem = null;
            Debug.Log("Dropped item");
        }
    }
}
