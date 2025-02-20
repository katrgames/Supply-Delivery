using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public Transform carryPosition; // A child object where the item will be carried
    private GameObject carriedItem; // The item the player is carrying

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from WASD keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        // Drop item with Space key
        if (Input.GetKeyDown(KeyCode.Space) && carriedItem != null)
        {
            DropItem();
        }
    }

    void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item") && carriedItem == null)
        {
            PickUpItem(collision.gameObject);
        }
    }

    private void PickUpItem(GameObject item)
    {
        carriedItem = item;
        item.transform.SetParent(carryPosition);
        item.transform.localPosition = Vector3.zero;

        // Change physics properties
        Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
        itemRb.bodyType = RigidbodyType2D.Kinematic; // Disable physics movement
        itemRb.linearVelocity = Vector2.zero; // Stop any existing movement
    }

    public void DropItem()
    {
        if (carriedItem != null)
        {
            Rigidbody2D itemRb = carriedItem.GetComponent<Rigidbody2D>();
            itemRb.bodyType = RigidbodyType2D.Dynamic; // Reactivate physics

            carriedItem.transform.SetParent(null);
            carriedItem = null;
        }
    }

    public GameObject GetCarriedItem()
    {
        return carriedItem;
    }
}
