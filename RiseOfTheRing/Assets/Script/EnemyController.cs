using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;             // Patrol speed of the enemy
    private int moveDirection = 1;           // 1 = right, -1 = left

    [Header("Ground Checking")]
    public float groundCheckDistance = 0.2f; // Raycast length downward
    public LayerMask groundLayer;            // LayerMask for ground/platform

    [Header("Turn Delay")]
    public float turnDelay = 0.5f;           // Time in seconds to wait before turning

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private bool isWaiting = false;          // Prevents multiple turn coroutines

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Prevent rotation on collisions
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (isWaiting)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Horizontal movement
        rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);

        // Check whether there is a cliff ahead. Uf it is almost at the cliff, start the delayed steering
        EdgeCheckAndTriggerDelay();
    }

    // Orifinal edge detecction: If there is no ground in fort, eait for turnDelay and turn around
    private void EdgeCheckAndTriggerDelay()
    {
        float halfWidth = col.bounds.extents.x;
        Vector2 origin = new Vector2(
            transform.position.x + halfWidth * moveDirection,
            transform.position.y - col.bounds.extents.y
        );
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, Color.red);

        if (hit.collider == null && !isWaiting)
        {
            StartCoroutine(WaitAndTurn());
        }
    }

    private IEnumerator WaitAndTurn()
    {
        isWaiting = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(turnDelay);

        ReverseDirection();  // Really turn around
        isWaiting = false;
    }

    // Flip the direction and adjust the sprite orientation
    private void ReverseDirection()
    {
        moveDirection *= -1;
        sr.flipX = (moveDirection < 0);
    }

    // When encounter a horizontal obstacle, turn around immediately
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Iterate through all collision points and check whether it's a horizontal collision
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // If the y component of the collision normal is very small (close to 0),
            // it's means that it's a collision in the left and right directions
            if (Mathf.Abs(contact.normal.y) < 0.1f)
            {
                
                // Make a turn for horizontal barriers only（normal.x ≈ ±1）
                ReverseDirection();
                break;
            }

           
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (col == null) return;

        float halfWidth = col.bounds.extents.x;
        Vector2 origin = new Vector2(
            transform.position.x + halfWidth * moveDirection,
            transform.position.y - col.bounds.extents.y
        );
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
    }
}
