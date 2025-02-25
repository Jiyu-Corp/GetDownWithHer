using UnityEngine;

public class ClimberEntity : Entity {
    public float climbSpeed = 3f;

    private bool canClimb = false;
    private bool isClimbing = false;

    public void StartClimb() {
        if(!canClimb) return;

        isClimbing = true;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
    }

    public void StopClimb() {
        if(!isClimbing) return;
        
        isClimbing = false;
        rb.gravityScale = 1f;
    }

    /// <summary>
    /// Climb up or down based on the direction param, need to be on isClimbing state
    /// </summary>
    /// <param name="verticalDirection">Value between -1 and 1.</param>
    public void Climb(float verticalDirection) {
        if(!isClimbing) return;

        rb.velocity = new Vector2(rb.velocity.x, verticalDirection * climbSpeed); 
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
        base.OnCollisionEnter2D(collision);

        const bool isCollisionClimbable = collision.gameObject.CompareTag("Climbable"); 
        if (isCollisionClimbable) {
            canClimb = true;
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision) {
        base.OnCollisionExit2D(collision); // Keep ground detection

        const bool isCollisionClimbable = collision.gameObject.CompareTag("Climbable"); 
        if (isCollisionClimbable) {
            canClimb = false;
        }
    }

}