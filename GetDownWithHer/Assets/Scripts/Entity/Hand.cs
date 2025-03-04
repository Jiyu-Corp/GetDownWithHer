using UnityEngine;

public class Hand : MonoBehaviour {
    public float handSpeed = 20f;

    private bool isHolding = false;
    private bool isPreparedToHold = false;

    private ClimberEntity climberEntity;
    private Rigidbody2D rb;
    private DistanceJoint2D dj;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        climberEntity = GetComponentInParent<ClimberEntity>();
        dj = GetComponent<DistanceJoint2D>();
    }

    void FixedUpdate() {
        Rigidbody2D rbCE = climberEntity.GetComponent<Rigidbody2D>();
        rb.linearVelocity = rbCE.linearVelocity;
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
        isPreparedToHold = enabled;
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