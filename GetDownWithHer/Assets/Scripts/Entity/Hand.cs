using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Hand : MonoBehaviour {
    [HideInInspector]
    public bool isHolding = false;
    
    private bool isPreparedToHold = false;

    private ClimberEntity climberEntity;
    private Rigidbody2D rb;
    private DistanceJoint2D dj;
    private Rigidbody2D parentRB;

    private List<Collision2D> currentCollisions = new();
    private float shoulderLimitDistance;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        climberEntity = GetComponentInParent<ClimberEntity>();
        dj = GetComponent<DistanceJoint2D>();
        parentRB = climberEntity.GetComponent<Rigidbody2D>();

        shoulderLimitDistance = dj.distance;

        dj.enabled = false;
    }

    void FixedUpdate() {
        ApplySpeed();
        HoldSurfaceIfCapable();
    }

    private void ApplySpeed() {
        Vector2 speed;
        Vector2 handPositionInWorldSpace = new Vector2(rb.transform.position.x, rb.transform.position.y);

        Vector2 vectorTowardsPointer = climberEntity.directionPointer - handPositionInWorldSpace;
        Vector2 speedTowardsPointer = (vectorTowardsPointer.magnitude > 0.05) ? vectorTowardsPointer.normalized * climberEntity.handSpeed : new Vector2(0,0);

        if(!climberEntity.isClimbing) {
            speed = parentRB.linearVelocity;

            speedTowardsPointer = AdjustSpeedToPointWithShoulderVirtualJoint(handPositionInWorldSpace, speedTowardsPointer);
            
            speed += speedTowardsPointer;
        } else speed = speedTowardsPointer;

        rb.linearVelocity = speed;
        Debug.Log("debug");
        Debug.Log(rb.linearVelocity);
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

    private void HoldSurfaceIfCapable() {
        if(!isHolding && isPreparedToHold && currentCollisions.Count > 0) Hold(); 
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
        currentCollisions.Add(collision);
    }

    void OnCollisionExit2D(Collision2D collision) {
        currentCollisions.Remove(currentCollisions.Last());
    }
}