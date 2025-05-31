// PlayerController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement & Jump Settings")]
    public float moveSpeed = 5f;              // Horizontal movement speed
    public float jumpForce = 10f;             // Force applied when jumping

    [Header("Boundary Limits")]
    public float minX = -2f;                  // Left boundary on X axis
    public float maxX = 2f;                   // Right boundary on X axis

    private Rigidbody2D rb;                   // Reference to the player's Rigidbody2D
    private bool isGrounded;                  // Flag indicating if the player is on the ground
    public LayerMask groundLayer;             // LayerMask used to identify ground objects

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Freeze rotation so the player doesn't rotate on collisions
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        // Horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Clamp player's X position within specified boundaries
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Jump when on the ground and the Jump button is pressed
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    /// <summary>
    /// Called when the player collides with a non-trigger Collider2D.
    /// Used to detect when the player is on the ground.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is part of the ground layer
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
        }
    }

    /// <summary>
    /// Called when the player exits a ground Collider2D.
    /// Sets isGrounded to false.
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    /// <summary>
    /// Called when the player enters a trigger Collider2D.
    /// Used to detect collision with enemies (which should have "Is Trigger" enabled).
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Notify GameManager that the player took damage
            GameManager.Instance.TakeDamage();
            // Destroy the enemy GameObject
            Destroy(other.gameObject);
        }
    }
}
