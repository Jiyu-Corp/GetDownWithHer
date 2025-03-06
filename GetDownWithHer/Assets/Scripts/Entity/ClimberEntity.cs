using UnityEngine;

using System;
using System.Threading.Tasks;

public class ClimberEntity : Entity {
    public readonly float handSpeed = 5f;

    private const int ID_L_HAND = 0;
    private const int ID_R_HAND = 1;

    [HideInInspector]
    public Vector2 directionPointer;

    GameObject lHand;
    Hand lHandScript;
    GameObject rHand;
    Hand rHandScript;

    [HideInInspector]
    public bool isClimbing = false;

    void Start() {
        lHand = transform.Find("LeftHand").gameObject;
        lHandScript = lHand.GetComponent<Hand>();
        rHand = transform.Find("RightHand").gameObject;
        rHandScript = rHand.GetComponent<Hand>();

        AdjustBodyColliders();
    }

    private void AdjustBodyColliders() {
        Collider2D collider = GetComponent<Collider2D>();
        Collider2D lHandCollider = lHand.GetComponent<Collider2D>();
        Collider2D rHandCollider = rHand.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(collider, lHandCollider);
        Physics2D.IgnoreCollision(collider, rHandCollider);
        Physics2D.IgnoreCollision(lHandCollider, rHandCollider);
    }

    public void SetDirectionPointer(Vector2 directionPointer) {
        this.directionPointer = directionPointer;
    }

    public void ManageHandsJoints(bool enableRequestFromHand) {
        bool enableHandsJoints = enableRequestFromHand || lHandScript.isHolding || rHandScript.isHolding;
        
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