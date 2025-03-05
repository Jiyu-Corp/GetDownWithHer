using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// HP still not implemented
public class Entity : MonoBehaviour {
    [Header("Movement Settings")]
    public float acceleration = 24;
    public float moveSpeed = 7f;
    public float boastedMoveSpeed = 14f;
    public float timeUntilReachBoastedSpeed = 2f;
    public float jumpForce = 7f;

    protected Rigidbody2D rb;

    // Terrain states
    protected bool inGround = false;
    protected bool inWall = false;

    protected bool isMoving = false;
    protected bool isBoastedMoving = false;
    private float movingDirection = 0f;
    private float timeSinceStartMoving = 0f;

    private Task cachedStartMove = null;
    private Task cachedStopMove = null;

    private Dictionary<Collider2D, Vector2> currentCollidingNormals = new Dictionary<Collider2D, Vector2>();

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    protected void FixedUpdate() {
        if(isMoving && !inWall) {
            isBoastedMoving = timeSinceStartMoving > timeUntilReachBoastedSpeed;

            rb.linearVelocity = new Vector2(Mathf.MoveTowards(
                rb.linearVelocity.x,
                movingDirection * (isBoastedMoving ? boastedMoveSpeed : moveSpeed),
                acceleration * Time.fixedDeltaTime
            ), rb.linearVelocity.y);

            timeSinceStartMoving += Time.fixedDeltaTime;
        } else if (inWall) {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        } else {
            timeSinceStartMoving = 0f;
            isBoastedMoving = false;

            rb.linearVelocity = new Vector2(Mathf.MoveTowards(
                rb.linearVelocity.x,
                0,
                acceleration/3 * Time.fixedDeltaTime
            ), rb.linearVelocity.y);
        }
    }

    public virtual void StartMove(float horizontalDirection) {
        bool isStartMoveSetupSuccessful = SetupStartMove(horizontalDirection);
        
        if(!isStartMoveSetupSuccessful) {
            if(cachedStopMove != null) cachedStopMove = null;
            cachedStartMove = StartMoveAsync(horizontalDirection);
        }
    }
    private bool SetupStartMove(float horizontalDirection) {
        if(inGround) {
            isMoving = true;
            movingDirection = horizontalDirection;
        }

        return inGround;
    }
    private Task StartMoveAsync(float horizontalDirection) {
        return Task.Run(() => {
            while(cachedStartMove != null) {
                bool isStartMoveSetupSuccessful = SetupStartMove(horizontalDirection);
                if(isStartMoveSetupSuccessful) cachedStartMove = null;

                Task.Yield();
            }
        });
    }

    public virtual void StopMove(bool verifyInGround = true) {
        bool isStopMoveSetupSuccessful = SetupStopMove(verifyInGround);

        if(!isStopMoveSetupSuccessful) {
            if(cachedStartMove != null) cachedStartMove = null;
            cachedStopMove = StopMoveAsync(verifyInGround);
        }
    }
    private bool SetupStopMove(bool verifyInGround) {
        if(!verifyInGround || inGround) {
            isMoving = false;
            movingDirection = 0f;
        }

        return !verifyInGround || inGround;
    }
    private Task StopMoveAsync(bool verifyInGround) {
        return Task.Run(() => {
            while(cachedStopMove != null) {
                bool isStopMoveSetupSuccessful = SetupStopMove(verifyInGround);
                if(isStopMoveSetupSuccessful) cachedStopMove = null;
                Task.Yield();
            }
        });
    }

    /// <summary>
    /// Moves the entity upwards the y axis if on ground
    /// </summary>
    public virtual void Jump() {
        if(inGround) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void ValidateAndUpdateTerrainStates() {
        float minYNormalForInGroundCheck = 0.3f;
        float minXNormalForInWallCheck = 0.75f;
        
        bool isInGroundDetected = false;
        bool isInWallDetected = false;
        foreach(Vector2 normal in currentCollidingNormals.Values) {
            if(!isInGroundDetected && normal.y >= minYNormalForInGroundCheck) {
                isInGroundDetected = true;
            }

            if(!isInWallDetected && Math.Abs(normal.x) >= minXNormalForInWallCheck) {
                isInWallDetected = true;
            }
        }

        inGround = isInGroundDetected;
        inWall = isInWallDetected;
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