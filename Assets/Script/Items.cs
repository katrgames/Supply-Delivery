using UnityEngine;
using System.Collections;

public class Items : MonoBehaviour
{
    public ItemsType type;
    public string description;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        StartCoroutine(ItemDropMovement());
    }

    IEnumerator ItemDropMovement()
    {
        // roll for item drop gravity
        float coordY = UnityEngine.Random.Range(-1f, 1f);
        float coordX = UnityEngine.Random.Range(-1f, 1f);

        Vector2 direction = new(coordX, coordY);

        // Set the speed of the drop
        //float dropSpeed = 1.0f;

        // Move the item using Transform.Translate
        /*float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            float moveAmount = dropSpeed * Time.deltaTime;
            transform.Translate(direction * moveAmount);
            elapsedTime += Time.deltaTime;
            yield return null;
        }*/
        float dropForce = 2.0f;
        rb.AddForce(direction * dropForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
    }
}
