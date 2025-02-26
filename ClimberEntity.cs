using UnityEngine;

public class ClimberEntity : Entity {
    private int stamina = 100;
    private bool isStaminaDraining = false;
    private int staminaDrainScale = 0;

    public float climbSpeed = 3f;

    private bool canClimb = false;
    private bool isClimbing = false;

    private bool isGeneratingRandomNextClimbStep = false;
    private ClimbStep? nextClimbStep = null;

    protected async void Start() {
        StartAutoStaminaRegenPerSecond();
    }

    private async void StartAutoStaminaRegenPerSecond() {
        while(true) {
            if(isStaminaDraining) continue;

            int delay = 1 * 1000;
            await Task.Delay(delay);
            
            stamina++;
        }
    }

    public void StartClimb() {
        if(!canClimb) return;

        isClimbing = true;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        StartGeneratingRandomNextClimbStep();
        StartStaminaDrainPerSecond();
    }

    public void StopClimb() {
        if(!isClimbing) return;
        
        isClimbing = false;
        rb.gravityScale = 1f;
        StopGeneratingRandomNextClimbStep();
        StopStaminaDrainPerSecond();
    }

    public ClimbStep GetNextClimbStep() {
        return nextClimbStep;
    }

    public ClimbStep VerifyAndResetNextClimbStep(ClimbStep inputedClimbStep) {
        ClimbStep correctClimbStep = nextClimbStep;
        nextClimbStep = null;

        if(nextClimbStep == inputedClimbStep) {
            staminaDrainScale--;
        } else {
            staminaDrainScale*=2;
        }

        if(stamina <= 0) StopClimb();
    }

    private async void StartGeneratingRandomNextClimbStep(int minDelay = 1000, int maxDelay = 3000) {
        isGeneratingRandomNextClimbStep = true;
        while(isGeneratingRandomNextClimbStep) {
            int delay = UnityEngine.Random.Range(minDelay, maxDelay);
            await Task.Delay(delay);
            if (!isGeneratingRandomNextClimbStep) return;

            nextClimbStep = GenerateRandomNextClimbStep()
        }
    }

    private void StopGeneratingRandomNextClimbStep() {
        isGeneratingRandomNextClimbStep = false;
    }

    private ClimbStep GenerateRandomNextClimbStep() {
        Array enumValues = Enum.GetValues(typeof(ClimbStep));
        
        ClimbStep randomStep = (ClimbStep)enumValues.GetValue(UnityEngine.Random.Range(0, enumValues.Length));
        
        return randomStep;
    }

    private async void StartStaminaDrainPerSecond() {
        isStaminaDraining = true;
        while(isStaminaDraining) {
            int delay = 1 * 1000;
            await Task.Delay(delay);
            
            staminaDrainScale++;
            stamina = stamina - staminaDrainScale;
        }
    }

    private void StopStaminaDrainPerSecond() {
        isStaminaDraining = false;
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