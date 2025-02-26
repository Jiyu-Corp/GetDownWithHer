using UnityEngine;

// Stamina and HP still not implemented
public class ClimberEntity : Entity {
    public float climbSpeed = 3f;

    private bool canClimb = false;
    private bool isClimbing = false;

    private bool isGeneratingRandomNextClimbStep = false;
    private ClimbStep? nextClimbStep = null;

    public void StartClimb() {
        if(!canClimb) return;

        isClimbing = true;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        StartGeneratingRandomNextClimbStep();
    }

    public void StopClimb() {
        if(!isClimbing) return;
        
        isClimbing = false;
        rb.gravityScale = 1f;
        StopGeneratingRandomNextClimbStep();
    }

    public ClimbStep GetNextClimbStep() {
        return nextClimbStep;
    }

    public ClimbStep VerifyNextClimbStep(ClimbStep inputedClimbStep) {
        // Stamina logic and etc
    }

    private async void StartGeneratingRandomNextClimbStep(int minDelay = 1000, int maxDelay = 3000) {
        isGeneratingRandomNextClimbStep = true;
        while(isGeneratingRandomNextClimbStep) {
            int delay = UnityEngine.Random.Range(minDelay, maxDelay);
            await Task.Delay(delay, token);
            if (!isGeneratingRandomNextClimbStep) return;

            nextClimbStep = GenerateRandomNextClimbStep()
        }
    }

    private void StopGeneratingRandomNextClimbStep() {
        isGeneratingRandomNextClimbStep = false;
    }

    public ClimbStep GenerateRandomNextClimbStep() {
        Array enumValues = Enum.GetValues(typeof(ClimbStep));
        
        ClimbStep randomStep = (ClimbStep)enumValues.GetValue(UnityEngine.Random.Range(0, enumValues.Length));
        
        return randomStep;
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
        base.OnCollisionExit2D(collision);

        const bool isCollisionClimbable = collision.gameObject.CompareTag("Climbable"); 
        if (isCollisionClimbable) {
            canClimb = false;
        }
    }

}