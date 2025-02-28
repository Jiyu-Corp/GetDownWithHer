using UnityEngine;

// HP still not implemented
public class Entity : MonoBehaviour {
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    protected Rigidbody2D rb;
    protected bool inGround = false;

    private float movingDirection = 0f;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    protected void FixedUpdate() {
        MoveEntity();
    }

    private void MoveEntity() {
        if (movingDirection != 0f && inGround) {
            rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, movingDirection * 14, 20 * Time.fixedDeltaTime), rb.linearVelocity.y);
            //rb.linearVelocity = new Vector2(movingDirection * moveSpeed, rb.linearVelocity.y);   
        }
    }

    /// <summary>
    /// Begin the movement of Entity in X axis
    /// </summary>
    /// <param name="horizontalDirection">Value between -1 and 1.</param>
    public virtual void BeginMove(float horizontalDirection) {
        if(inGround) {
            movingDirection = horizontalDirection;
        }
    }

    public virtual void StopMove() {
        if(inGround) {
            movingDirection = 0f;
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