using UnityEngine;

// HP still not implemented
public class Entity : MonoBehaviour {
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    protected Rigidbody2D rb;
    protected bool inGround = false;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Moves the Entity horizontally if on ground
    /// </summary>
    /// <param name="horizontalDirection">Value between -1 and 1.</param>
    public virtual void Move(float horizontalDirection) {
        if(inGround) {
            rb.linearVelocity = new Vector2(horizontalDirection * moveSpeed, rb.linearVelocity.y);    
        }
    }

    /// <summary>
    /// Moves the entity upwards the y axis if on ground
    /// </summary>
    public virtual void Jump() {
        if(inGround) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        bool isGroundCollision = collision.gameObject.layer == LayerMask.NameToLayer("Ground"); // if Entity touch the colisor with head, it would make it possible to walk... FIX THAT

        if(isGroundCollision) {
            inGround = true;
        }
    }
    protected virtual void OnCollisionExit2D(Collision2D collision) {
        bool isGroundCollision = collision.gameObject.layer == LayerMask.NameToLayer("Ground");

        if (isGroundCollision) {
            inGround = false;
        }
    }
}