using UnityEngine;

public class Hand : MonoBehaviour {
    public float handSpeed = 5f;

    private bool isHolding = false;
    private bool isPreparedToHold = false;

    private ClimberEntity climberEntity;
    protected Rigidbody2D rb;

    void Start() {
        climberEntity = GetComponentInParent<ClimberEntity>();
    }

    public void MoveHandToPosition(Vector2 position) {
        if(isHolding) return;

        // proper validate if Distance Joint will pull the binded object

        Vector2 direction = position - rb.position;
        Vector2 velocityOfMovement = Vector2.MoveTowards(rb.velocity, direction.normalized * handSpeed, handSpeed * Time.deltaTime);

        rb.velocity = velocityOfMovement;
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