using UnityEngine;

using System;
using System.Threading.Tasks;

public class ClimberEntity : Entity {
    [Header("Stamina Settings")]
    public const float MAX_STAMINA = 100f;
    public const float STAMINA_OFFSET_PER_SECOND = 10f;
    public const float STAMINA_LOSE_VALUE_CAUSE_STRUCTURE_HOLD = 5f;

    public readonly float handSpeed = 5f;

    private float stamina = MAX_STAMINA;

    private const int ID_L_HAND = 0;
    private const int ID_R_HAND = 1;

    [HideInInspector]
    public Vector2 directionPointer;

    protected GameObject lHand;
    protected Hand lHandScript;
    protected Collider2D lHandCollider;
    protected GameObject rHand;
    protected Hand rHandScript;
    protected Collider2D rHandCollider;

    [HideInInspector]
    public bool isClimbing = false;

    void Start() {
        lHand = transform.Find("LeftHand").gameObject;
        lHandScript = lHand.GetComponent<Hand>();
        lHandCollider = lHand.GetComponent<Collider2D>();
        
        rHand = transform.Find("RightHand").gameObject;
        rHandScript = rHand.GetComponent<Hand>();
        rHandCollider = rHand.GetComponent<Collider2D>();

        AdjustBodyColliders();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        ManageStamina();
        if(stamina <= 0) DisableClimb();
    }

    public float GetStamina() {
        return stamina;
    }

    private void ManageStamina() {
        bool isStaminaOffsetNeeded = isClimbing && stamina > 0 || !isClimbing && stamina < 100;
        if(!isStaminaOffsetNeeded) return;

        float staminaOffset = STAMINA_OFFSET_PER_SECOND * Time.fixedDeltaTime * (isClimbing ? -1 : 1);
        
        stamina += staminaOffset;
    }

    private void DisableClimb() {
        ManageHoldHand(ID_L_HAND, false);
        ManageHoldHand(ID_R_HAND, false);
    }

    private void DecreaseStaminaCauseStructureHold() {
        stamina -= STAMINA_LOSE_VALUE_CAUSE_STRUCTURE_HOLD;
    }

    private void AdjustBodyColliders() {
        Physics2D.IgnoreCollision(cld, lHandCollider);
        Physics2D.IgnoreCollision(cld, rHandCollider);
        Physics2D.IgnoreCollision(lHandCollider, rHandCollider);
    }

    public void SetDirectionPointer(Vector2 directionPointer) {
        this.directionPointer = directionPointer;
    }

    public void ManageHandsJoints(bool enableRequestFromHand) {
        bool enableHandsJoints = enableRequestFromHand || lHandScript.isHolding || rHandScript.isHolding;
        
        if(enableRequestFromHand) DecreaseStaminaCauseStructureHold();

        lHandScript.ManageJoint(enableHandsJoints);
        rHandScript.ManageJoint(enableHandsJoints);

        isClimbing = enableHandsJoints;
    }
    public void ManageHoldHand(int idHand, bool enable) {
        Hand handSelected = idHand == ID_L_HAND
            ?   lHandScript
            :   rHandScript;


        handSelected.PrepareHold(enable);
        if(!enable && handSelected.isHolding) handSelected.StopHold();
    }
}