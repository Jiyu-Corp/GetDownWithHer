using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 7f;

    protected Rigidbody2D rb;

    private float horizontalMoveDirection = 0f;

    private Dictionary<Collider2D, Vector2> currentCollidingNormals = new Dictionary<Collider2D, Vector2>();

    // Terrain states
    private bool inGround = false;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    protected void FixedUpdate() {
        PlayerMove();
    }

    private void PlayerMove() {
        bool canPlayerChangeEntitySpeed = horizontalMoveDirection != 0f && inGround;
        if(canPlayerChangeEntitySpeed) {
            const float defaultAcceleration = 16f;

            rb.linearVelocity = new Vector2(Mathf.MoveTowards(
                rb.linearVelocity.x,
                horizontalMoveDirection * moveSpeed,
                defaultAcceleration * Time.fixedDeltaTime
            ), rb.linearVelocity.y);
        }
    }

    public virtual void Jump() {
        if(inGround) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void ComposeMoveDirection(float direction, bool addDirection) {
        horizontalMoveDirection += direction * (addDirection ? 1 : -1);
    }

    protected void ValidateAndUpdateTerrainStates() {
        float minYNormalForInGroundCheck = 0.3f;
        
        bool isInGroundDetected = false;
        foreach(Vector2 normal in currentCollidingNormals.Values) {
            if(!isInGroundDetected && normal.y >= minYNormalForInGroundCheck) {
                isInGroundDetected = true;
            }
        }

        inGround = isInGroundDetected;
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        foreach(ContactPoint2D contact in collision.contacts) {
            currentCollidingNormals[contact.collider] = contact.normal;
        }

        ValidateAndUpdateTerrainStates();
    }
    protected virtual void OnCollisionExit2D(Collision2D collision) {
        currentCollidingNormals.Remove(collision.collider);

        ValidateAndUpdateTerrainStates();
    }
}