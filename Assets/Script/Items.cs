using System.Collections;
using TMPro;
using UnityEngine;

public class Items : MonoBehaviour
{
    public ItemsType type;
    public string description;
    public GameObject shadow;

    [Header("Move Animation Settings")]
    public float grabAnimSpeed = 0.2f; // Time to move away
    public EasingFunction.Ease grabAnimEase = EasingFunction.Ease.EaseOutQuad; // Easing for movement

    [Header("Scale Animation Settings")]
    public float spawnScale = 1.5f; // Scale size increase
    public float spawnDuration = 0.5f; // Time to scale up

    public float spawnDistance = 0.5f; // How far it moves while spawning
    public EasingFunction.Ease spawnEase = EasingFunction.Ease.EaseOutBack; // Easing for scaling
    public EasingFunction.Ease moveEaseType = EasingFunction.Ease.EaseOutQuad; // Easing for movement
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private Rigidbody2D rb;
    private Collider2D col;
    private Transform followTarget;

    private Vector2 originalScale;
    public bool isSpawning = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        startPosition = transform.position;
        targetPosition = startPosition + GetRandomDirection() * spawnDistance;
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero; // Start from zero size
        StartCoroutine(SpawnWithBounce());
    }

    Vector3 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;
    }

    IEnumerator SpawnWithBounce()
    {
        isSpawning = true;
        float elapsedTime = 0f;
        var easingFunction = EasingFunction.GetEasingFunction(spawnEase);
        var moveEasing = EasingFunction.GetEasingFunction(moveEaseType);

        while (elapsedTime < spawnDuration)
        {
            float t = elapsedTime / spawnDuration;
            float easedT = easingFunction(0f, 1f, t); // Get eased interpolation value
            float moveT = moveEasing(0f, 1f, t); // Movement easing

            // Apply scale with bounce effect
            float scaleMultiplier = Mathf.Lerp(0f, spawnScale, easedT);
            transform.localScale = originalScale * scaleMultiplier;

            // Move in a random direction with easing
            transform.position = Vector3.Lerp(startPosition, targetPosition, moveT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the scale is set correctly at the end
        transform.localScale = originalScale;
        isSpawning = false;
    }

    public void GrabItem(Transform holdPoint)
    {
        StopAllCoroutines(); // Stop any running animations
        followTarget = holdPoint;
        GameManager.instance.ChangeItemDescText(description);

        rb.simulated = false;
        col.enabled = false;
        // Move to hold position first
        StartCoroutine(
            MoveAnimation(
                transform.position,
                holdPoint.position,
                grabAnimSpeed,
                EasingFunction.Ease.EaseOutQuad
            )
        );
        shadow.SetActive(false);
        // Start following coroutine
        StartCoroutine(FollowHoldPoint());
    }

    private IEnumerator FollowHoldPoint()
    {
        while (followTarget != null)
        {
            // Smoothly follow holdPoint
            transform.position = Vector3.Lerp(
                transform.position,
                followTarget.position,
                Time.deltaTime * 30f
            );
            yield return null; // Wait for the next frame
        }
    }

    public void DropItem(Vector3 dropPoint)
    {
        StopAllCoroutines(); // Stop following when dropped
        GameManager.instance.ChangeItemDescText("");
        followTarget = null;
        rb.simulated = true;
        col.enabled = true;
        shadow.SetActive(true);
        StartCoroutine(MoveAnimation(transform.position, dropPoint, grabAnimSpeed, grabAnimEase));
    }

    private IEnumerator MoveAnimation(
        Vector3 start,
        Vector3 end,
        float duration,
        EasingFunction.Ease easeType
    )
    {
        float elapsedTime = 0f;
        var easingFunction = EasingFunction.GetEasingFunction(easeType);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector2 newPosition = new Vector2(
                easingFunction(start.x, end.x, t),
                easingFunction(start.y, end.y, t)
            );

            rb.MovePosition(newPosition); // Move using Rigidbody2D for physics-friendly movement

            elapsedTime += Time.deltaTime;
            yield return null;
            Debug.Log("Move to: " + newPosition);
        }

        rb.MovePosition(end); // Ensure exact final position
    }
}
