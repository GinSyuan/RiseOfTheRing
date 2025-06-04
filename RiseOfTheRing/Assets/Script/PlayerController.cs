using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement & Jump Settings")]
    public float moveSpeed = 5f;      // Horizontal movement speed
    public float jumpForce = 10f;     // Upward force applied when jumping
    public float deadZone = 0.2f;     // Input dead zone threshold to avoid tiny inputs

    [Header("Knockback & Invincibility")]
    public float knockbackForce = 5f; // Force applied to the player when bumped by an enemy
    public float invincibleTime = 1f; // Duration of invincibility after taking damage
    public int flashCount = 3;        // Number of times to flash the player sprite (red/normal)

    [Header("Boundary Limits")]
    public float minX = -2f;          // Left horizontal boundary
    public float maxX = 2f;           // Right horizontal boundary

    [Header("Ground Detection")]
    public LayerMask groundLayer;     // Layer(s) considered as ground for collision checks

    private Rigidbody2D rb;           // Reference to Rigidbody2D component
    private bool isGrounded;          // True when player is on the ground
    private SpriteRenderer sr;        // Reference to SpriteRenderer for flashing effect
    private Color originalColor;      // Original sprite color to restore after flashing
    private bool isInvincible = false;// Flag to prevent damage while invincible
    private Animator animator;        // Reference to Animator for controlling animations

    private void Awake()
    {
        // Cache components
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation on collisions

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color; // Save the default sprite color

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get horizontal input from keyboard or controller
        float rawInput = Input.GetAxis("Horizontal");
        // Apply dead zone: if absolute value < deadZone, treat as zero
        float moveInput = Mathf.Abs(rawInput) < deadZone ? 0f : rawInput;

        // Animate walking/running by setting "Speed" parameter (0..1)
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Apply horizontal movement
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Clamp the player's X position within specified boundaries
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Handle jumping: only if on the ground and jump button pressed
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // Apply upward velocity for jump
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("IsJumping", true); // Trigger jump animation

            // Play jump sound via AudioManager
            AudioManager.Instance.PlayJumpSound();
        }
    }

    private void FixedUpdate()
    {
        // If the player is in the air and moving upward, keep the jump animation active
        if (!isGrounded && rb.velocity.y > 0.1f)
        {
            animator.SetBool("IsJumping", true);
        }
    }


    /// Called when the player collides (non-trigger) with another Collider2D.
    /// Handles ground detection and enemy collisions (stomp vs side hit).
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided object is ground by matching its layer to groundLayer
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false); // Stop jump animation on landing
        }

        // Check if collided object is tagged "Enemy"
        if (collision.collider.CompareTag("Enemy"))
        {
            if (isInvincible)
                return; // Ignore collisions while invincible

            // Determine if the player landed on top of the enemy (stomp)
            bool stomped = false;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    stomped = true;
                    break;
                }
            }

            if (stomped)
            {
                // Player stomped enemy: bounce up and destroy the enemy GameObject
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.8f);
                Destroy(collision.collider.gameObject);
            }
            else
            {
                // Side collision: apply knockback, decrease life, play damage sound, and start invincibility
                Vector2 knockDir = (transform.position.x < collision.transform.position.x)
                    ? Vector2.left
                    : Vector2.right;
                ApplyKnockback(knockDir);

                // Notify GameManager to reduce player's life count
                GameManager.Instance.TakeDamage();

                // Play damage SFX via AudioManager
                AudioManager.Instance.PlayDamageSound();

                // Start flashing invincibility coroutine
                StartCoroutine(InvincibilityAndFlash());
            }
        }
    }


    /// Called when the player exits collision with a Collider2D.
    /// Used here to unset isGrounded when leaving a ground platform.
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    
    /// Apply knockback force to the player in the specified horizontal direction.
    private void ApplyKnockback(Vector2 direction)
    {
        rb.velocity = new Vector2(direction.x * knockbackForce, rb.velocity.y * 0.5f);
    }


    /// Coroutine that makes the player temporarily invincible and flashes the sprite red/normal.
    private IEnumerator InvincibilityAndFlash()
    {
        isInvincible = true;

        // Calculate interval between color changes
        float flashInterval = invincibleTime / (flashCount * 2f);

        for (int i = 0; i < flashCount; i++)
        {
            if (sr != null)
                sr.color = Color.red; // Change sprite color to red
            yield return new WaitForSeconds(flashInterval);

            if (sr != null)
                sr.color = originalColor; // Restore original color
            yield return new WaitForSeconds(flashInterval);
        }

        isInvincible = false; // End invincibility
    }
}
