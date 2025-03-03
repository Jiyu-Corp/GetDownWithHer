using UnityEngine;

using System;
using System.Threading.Tasks;

public class ClimberEntity : Entity {
    GameObject lHand;
    GameObject rHand;

    void Start() {
        lHand = transform.Find("LeftHand").gameObject;
        rHand = transform.Find("RightHand").gameObject;

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

    public void MoveHands(Vector2 position) {
        Hand lHandScript = lHand.GetComponent<Hand>();
        Hand rHandScript = rHand.GetComponent<Hand>();

        lHandScript.MoveHandToPosition(position);
        rHandScript.MoveHandToPosition(position);
    }
}