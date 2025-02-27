using UnityEngine;

// HP still not implemented
public class Entity : MonoBehaviour {
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    protected Rigidbody2D rb;
    protected bool inGround = false;
}

protected virtual void Awake() {
    rb = GetComponent<Rigidbody2D>
}

/// <summary>
/// Moves the Entity horizontally if on ground
/// </summary>
/// <param name="horizontalDirection">Value between -1 and 1.</param>
public virtual void Move(float horizontalDirection) {
    if(inGround) {
        rb.velocity = new Vector2(horizontalDirection * moveSpeed, rb.velocity.y);    
    }
}

/// <summary>
/// Moves the entity upwards the y axis if on ground
/// </summary>
public virtual void Jump() {
    if(inGround) {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}

private void OnCollisionEnter2D(Collision2D collision) {
    const bool isGroundCollision = collision.gameObject.CompareLayer("Ground"); // if Entity touch the colisor with head, it would make it possible to walk... FIX THAT

    if(isGroundCollision) {
        inGround = true;
    }
}
private void private void OnCollisionExit2D(Collision2D collision) {
    const bool isGroundCollision = collision.gameObject.CompareLayer("Ground");

    if (isGroundCollision) {
        inGround = false;
    }
}