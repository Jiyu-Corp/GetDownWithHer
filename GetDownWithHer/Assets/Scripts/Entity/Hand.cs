using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Hand : MonoBehaviour {
    public float handSpeed = 20f;

    private bool isHolding = false;
    private bool isPreparedToHold = false;

    private ClimberEntity climberEntity;
    private Rigidbody2D rb;
    private DistanceJoint2D dj;

    private Vector2 directionPointer; // Where mouse is, maybe should be on another script? Maybe Climb?

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        climberEntity = GetComponentInParent<ClimberEntity>();
        dj = GetComponent<DistanceJoint2D>();
    }

    void FixedUpdate() {
        ApplySpeed();
    }

    private void ApplySpeed() {
        Vector2 speed = new Vector2(0,0);
        if(!isHolding && IsOnShoulderLimitDistance()) { // the isHolding apply to both hands, in other words, it needs to be on the entity, just like this here we would know if the other hand is holding
            // Works on directionPointer, mensurate its true module, they parse it removing the cos of the module
        } else {
            // just apply the direction pointer speed, cause it is holding or not on limit
        }

        if(!isHolding) { //To make the hand follow the body, to avoid inertia to stop it
            Rigidbody2D rbCE = climberEntity.GetComponent<Rigidbody2D>();
            speed += rbCE.linearVelocity;
        }

        rb.linearVelocity = speed;
    }

    public void MoveHandToPosition(Vector2 position) {
        return;
        // if(isHolding) return;

        // // proper validate if Distance Joint will pull the binded object

        // Vector2 direction = position - rb.position;
        // Debug.Log(direction);
        // Vector2 velocityOfMovement = Vector2.MoveTowards(rb.linearVelocity, direction.normalized * handSpeed, handSpeed * Time.deltaTime);

        // rb.linearVelocity = velocityOfMovement;
    }

    public void PrepareHold(bool enable) {
        isPreparedToHold = enable;
    }

    public void StopHold() {
        isHolding = false;
    }

    public void Hold() {
        isHolding = true;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(isHolding) return;
        if(!isPreparedToHold) return;

        Hold();
    }
}