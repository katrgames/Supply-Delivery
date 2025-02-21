using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    [Header("Item Interaction")]
    private Items heldItem;
    public Transform holdPoint; // Position where the item will be held
    public Transform dropPoint;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input from WASD keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
        if (movement.magnitude > 0)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }
        // Handle grabbing and dropping items
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (heldItem == null) // Grab an item
            {
                Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, 1f);
                foreach (var itemCollider in items)
                {
                    Items item = itemCollider.GetComponent<Items>();
                    if (item != null)
                    {
                        heldItem = item;
                        heldItem.GrabItem(holdPoint); // Call GrabItem from Items.cs
                        anim.SetTrigger("Take");
                        break;
                    }
                }
            }
            else // Drop the item
            {
                heldItem.DropItem(dropPoint.position); // Call DropItem from Items.cs
                anim.SetTrigger("Take");
                heldItem = null;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput = moveInput.normalized;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, movement * moveSpeed, 0.1f);
    }

    //private void GrabItem()
    //{
    //    if (heldItem == null)
    //    {
    //        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f); // Detect nearby items
    //        foreach (Collider2D col in colliders)
    //        {
    //            if (col.CompareTag("Item"))
    //            {
    //                heldItem = col.GetComponent<Items>();
    //                if (heldItem != null)
    //                {
    //                    heldItem.transform.SetParent(holdPoint);
    //                    heldItem.transform.localPosition = Vector3.zero;
    //                    heldItem.GetComponent<Rigidbody2D>().simulated = false; // Disable physics
    //                    heldItem.GetComponent<Collider2D>().enabled = false;
    //                    Debug.Log("Picked up: " + heldItem.name);
    //                    return;
    //                }
    //            }
    //        }
    //    }
    //}

    //private void DropItem()
    //{
    //    if (heldItem != null)
    //    {
    //        heldItem.transform.SetParent(null);
    //        heldItem.GetComponent<Rigidbody2D>().simulated = true; // Enable physics
    //        heldItem.GetComponent<Collider2D>().enabled = true;
    //        heldItem.StartCoroutine(heldItem.ScaleOnlyAnimation(heldItem.transform.localScale, 1.5f, 0.5f, 1f, EasingFunction.Ease.EaseOutQuad, EasingFunction.Ease.EaseOutBounce));
    //        heldItem = null;
    //        Debug.Log("Dropped item");
    //    }
    //}
}
