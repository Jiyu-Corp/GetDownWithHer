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

    private float shoulderLimitDistance = 1f;
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
        Vector2 speed;

        Vector2 speedTowardsPointer = Vector2.MoveTowards(rb.position, directionPointer, handSpeed * Time.deltaTime);

        if(!isHolding) {
            Rigidbody2D parentRB = climberEntity.GetComponent<Rigidbody2D>();
            speed = parentRB.linearVelocity;

            if(IsHandOnShoulderLimit()) {
                Vector2 shoulderJointPoint = dj.connectedAnchor;
                Vector2 vectorFromHandToShoulder = shoulderJointPoint - rb.position;
                float distanceToShoulderLimit = vectorFromHandToShoulder.magnitude - shoulderLimitDistance;
                Vector2 vectorToShoulderLimit = vectorFromHandToShoulder.normalized * distanceToShoulderLimit;
                Vector2 pointOfShoulderLimit = rb.position + vectorToShoulderLimit;

                Vector2 velocityToApproachLimit = Vector2.MoveTowards(rb.position, pointOfShoulderLimit, Mathf.Infinity * Time.fixedDeltaTime);
                speed += velocityToApproachLimit;
            } 
            
            speed += speedTowardsPointer;
        } else {
            speed = speedTowardsPointer;
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