using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float airControlFactor = 0.5f;

    private Rigidbody2D rb;
    private Collider2D col;
    private float horizontalMovement;
    private bool jumpPressed;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        // Check if the player is grounded
        isGrounded = Physics2D.Raycast(col.bounds.center - new Vector3(0f, col.bounds.extents.y, 0f), Vector2.down, groundCheckDistance, groundLayer);
        bool isTouchingWall = Physics2D.Raycast(col.bounds.center, Vector2.right, col.bounds.extents.x + 0.1f, groundLayer) || Physics2D.Raycast(col.bounds.center, Vector2.left, col.bounds.extents.x + 0.1f, groundLayer);
        if (!isGrounded && isTouchingWall)
        {
            isGrounded = true;
        }

        Vector2 velocity = rb.velocity;

        // Horizontal movement
        velocity.x = horizontalMovement;

        // Jumping
        if (jumpPressed && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
        }

        // Air control
        if (!isGrounded)
        {
            velocity.x *= airControlFactor;
        }

        // Set the velocity of the Rigidbody2D component
        rb.velocity = velocity;

        // Flip the player's sprite
        if (horizontalMovement > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (horizontalMovement < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        // Reset the jumpPressed flag
        jumpPressed = false;
    }

}