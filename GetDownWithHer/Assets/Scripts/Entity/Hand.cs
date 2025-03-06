using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Hand : MonoBehaviour {
    public float handSpeed = 20f;

    public bool isHolding = false;
    private bool isPreparedToHold = false;

    private ClimberEntity climberEntity;
    private Rigidbody2D rb;
    private DistanceJoint2D dj;
    private Rigidbody2D parentRB;

    private readonly float shoulderLimitDistance = 1f;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        climberEntity = GetComponentInParent<ClimberEntity>();
        dj = GetComponent<DistanceJoint2D>();
        parentRB = climberEntity.GetComponent<Rigidbody2D>();

        dj.enabled = false;
    }

    void FixedUpdate() {
        ApplySpeed();
    }

    private void ApplySpeed() {
        Vector2 speed;
        Vector2 handPositionInWorldSpace = new Vector2(rb.transform.position.x, rb.transform.position.y);

        Vector2 vectorTowardsPointer = climberEntity.directionPointer - handPositionInWorldSpace;
        Vector2 speedTowardsPointer = (vectorTowardsPointer.magnitude > 0.05) ? vectorTowardsPointer.normalized * handSpeed : new Vector2(0,0);

        if(!isHolding) {
            speed = parentRB.linearVelocity;

            speedTowardsPointer = AdjustSpeedToPointWithShoulderVirtualJoint(handPositionInWorldSpace, speedTowardsPointer);
            
            speed += speedTowardsPointer;
        } else speed = speedTowardsPointer;

        rb.linearVelocity = speed;
    }

    private Vector2 AdjustSpeedToPointWithShoulderVirtualJoint(Vector2 handPosition, Vector2 originalSpeed) {
        Vector2 shoulderJointPointInWorldSpace = dj.connectedAnchor + parentRB.position;
        Vector2 nVectorFromHandToShoulder = (shoulderJointPointInWorldSpace - handPosition).normalized;
        float projectionLength =  Vector2.Dot(originalSpeed, nVectorFromHandToShoulder);
        if(IsHandOnShoulderLimit(handPosition) && projectionLength < 0) {
            Vector2 projectionFromHandToShoulderOnPointerSpeed = projectionLength * nVectorFromHandToShoulder;
            Vector2 limitedSpeed = originalSpeed - projectionFromHandToShoulderOnPointerSpeed;

            return limitedSpeed;
        }

        return originalSpeed;
    }

    private bool IsHandOnShoulderLimit(Vector2 handPosition) {
        Vector2 shoulderJointPoint = dj.connectedAnchor + parentRB.position;
        float distanceBetweenHandAndShoulder = Vector2.Distance(shoulderJointPoint, handPosition);
        return distanceBetweenHandAndShoulder >= shoulderLimitDistance;
    }

    public void PrepareHold(bool enable) {
        isPreparedToHold = enable;
    }

    public void StopHold() {
        isHolding = false;

        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;

        climberEntity.ManageHandsJoints(false);
    }

    public void Hold() {
        isHolding = true;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
        
        climberEntity.ManageHandsJoints(true);
    }
    
    public void ManageJoint(bool enable) {
        dj.enabled = enable;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(isHolding) return;
        if(!isPreparedToHold) return;

        Hold();
    }
}