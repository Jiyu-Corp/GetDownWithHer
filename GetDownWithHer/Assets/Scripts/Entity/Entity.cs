using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {
    [Header("HP Settings")]
    public const float MAX_HP = 100f;
    public const float HP_HEAL_PER_SECOND = 5f;
    public const float MIN_SPEED_FALL_TO_DAMAGE = 10f;
    public const float DAMAGE_PER_FALL_SPEED_PER_SECOND = 10f;

    private float hp = MAX_HP;

    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 7f;

    protected Rigidbody2D rb;
    protected Collider2D cld;

    private float horizontalMoveDirection = 0f;

    private Dictionary<Collider2D, Vector2> currentCollidingNormals = new Dictionary<Collider2D, Vector2>();

    // Terrain states
    protected bool inGround = false;

    private float fallSpeedOfTheLastUpd = 0f;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        cld = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate() {
        PlayerMove();
        StoreFallSpeed();
        HealHp();
    }

    protected virtual void HealHp() {
        bool isHpHealNeeded = hp < 100;
        if(!isHpHealNeeded) return;

        hp += HP_HEAL_PER_SECOND * Time.fixedDeltaTime;
    }

    private void StoreFallSpeed() {
        fallSpeedOfTheLastUpd = inGround
            ?   0
            :   -rb.linearVelocityY;
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

    public float GetHp() {
        return hp;
    }

    private void CalculateFallDamage() {
        if(fallSpeedOfTheLastUpd < MIN_SPEED_FALL_TO_DAMAGE) return;

        float hpToLose = fallSpeedOfTheLastUpd * DAMAGE_PER_FALL_SPEED_PER_SECOND;

        hp -= hpToLose;
        if(hp <= 0) Die();
    }

    protected virtual void Die() {}

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
        if(inGround) CalculateFallDamage();
    }
    protected virtual void OnCollisionExit2D(Collision2D collision) {
        currentCollidingNormals.Remove(collision.collider);

        ValidateAndUpdateTerrainStates();
    }
}