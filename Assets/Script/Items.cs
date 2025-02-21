using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;


public class Items : MonoBehaviour
{
    public ItemsType type;
    public string description;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isFollowing = false;
    private Transform followTarget;

    [Header("Move Animation Settings")]
    public float moveAwayDistance = 1f; // Distance to move away
    public float moveAwayTime = 0.5f; // Time to move away
    public EasingFunction.Ease moveEase = EasingFunction.Ease.EaseOutQuad; // Easing for movement

    [Header("Scale Animation Settings")]
    public float scaleIncrease = 1.5f; // Scale size increase
    public float scaleTime = 0.5f; // Time to scale up
    public float shrinkTime = 1f; // Time to shrink back
    public EasingFunction.Ease scaleEase = EasingFunction.Ease.EaseOutBounce; // Easing for scaling

    private Vector3 initialPosition;
    private Vector3 moveAwayTarget; // The random movement direction
    private Vector3 originalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get Rigidbody2D component
        initialPosition = transform.position;
        originalScale = transform.localScale;
        // Generate a random direction
        Vector2 randomDirection = Random.insideUnitCircle.normalized; // Random direction in 2D space
        Vector3 moveAwayTarget = transform.position + (Vector3)randomDirection;

        PlayFullSpawnAnimation(); // Play full animation when spawned
    }

    private void Update()
    {
        
    }

    public void PlayFullSpawnAnimation()
    {
        StopAllCoroutines(); // Stop any existing animation
        initialPosition = transform.position;

        // Generate a new random direction
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        moveAwayTarget = initialPosition + new Vector3(randomDirection.x, randomDirection.y, 0) * moveAwayDistance;

        StartCoroutine(SpawnAnimation(transform.position, moveAwayTarget, 1f, transform.localScale, 1.2f, 0.3f, 0.5f, EasingFunction.Ease.EaseOutQuad, EasingFunction.Ease.EaseOutBounce));
    }

    //public void PlayScaleAnimation()
    //{
    //    StopAllCoroutines(); // Stop any existing animation
    //    StartCoroutine(ScaleOnlyAnimation(transform.localScale, 1.2f, 0.3f, 0.5f, EasingFunction.Ease.EaseOutQuad, EasingFunction.Ease.EaseOutBounce));
    //}
    private IEnumerator SpawnAnimation(Vector3 startLocation, Vector3 endLocation, float moveTime, Vector3 startScale, float targetScale, float scaleT, float shrinkT, EasingFunction.Ease startEase, EasingFunction.Ease endEase)
    {
        // Step 1: Move away from spawn point in a random direction
        yield return MoveAnimation(startLocation, endLocation, moveTime, startEase);

        // Step 2: Scale up and bounce back to normal
        Vector3 targetIncrease = startScale * targetScale;
        yield return ScaleAnimation(startScale, targetIncrease, scaleT, startEase);
        yield return ScaleAnimation(targetIncrease, startScale, shrinkT, endEase);
    }
    public IEnumerator ScaleOnlyAnimation(Vector3 startScale, float targetScale, float scaleT, float shrinkT, EasingFunction.Ease startEase, EasingFunction.Ease endEase)
    {
        Vector3 targetIncrease = startScale * targetScale;
        yield return ScaleAnimation(startScale, targetIncrease, scaleT, startEase);
        yield return ScaleAnimation(targetIncrease, startScale, shrinkT, endEase);
    }
    public void GrabItem(Transform holdPoint)
    {
        StopAllCoroutines(); // Stop any running animations
        followTarget = holdPoint;
        isFollowing = true;

        rb.simulated = false;
        col.enabled = false;
        // Move to hold position first
        StartCoroutine(MoveAnimation(transform.position, holdPoint.position, 0.2f, EasingFunction.Ease.EaseOutQuad));

        // Start following coroutine
        StartCoroutine(FollowHoldPoint());
    }

    private IEnumerator FollowHoldPoint()
    {
        while (isFollowing && followTarget != null)
        {
            // Smoothly follow holdPoint
            transform.position = Vector3.Lerp(transform.position, followTarget.position, Time.deltaTime * 15f);
            yield return null; // Wait for the next frame
        }
    }

    public void DropItem(Vector3 dropPoint)
    {
        StopAllCoroutines(); // Stop following when dropped
        isFollowing = false;
        followTarget = null;
        rb.simulated = true;
        col.enabled = true;
        StartCoroutine(SpawnAnimation(transform.position, dropPoint, 1f,transform.localScale, 1.2f, 0.4f, 0.2f,EasingFunction.Ease.EaseOutQuad, EasingFunction.Ease.EaseOutBounce));
    }

    private IEnumerator MoveAnimation(Vector3 start, Vector3 end, float duration, EasingFunction.Ease easeType)
    {
        float elapsedTime = 0f;
        var easingFunction = EasingFunction.GetEasingFunction(easeType);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector3 newPosition = new Vector3(
                easingFunction(start.x, end.x, t),
                easingFunction(start.y, end.y, t)
                //easingFunction(start.z, end.z, t)
            );

            rb.MovePosition(newPosition); // Move using Rigidbody2D for physics-friendly movement

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(end); // Ensure exact final position
    }
    private IEnumerator ScaleAnimation(Vector3 start, Vector3 end, float duration, EasingFunction.Ease easeType)
    {
        float elapsedTime = 0f;
        var easingFunction = EasingFunction.GetEasingFunction(easeType);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localScale = new Vector3(
                easingFunction(start.x, end.x, t),
                easingFunction(start.y, end.y, t),
                easingFunction(start.z, end.z, t)
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = end; // Ensure final scale is exact
    }
}


