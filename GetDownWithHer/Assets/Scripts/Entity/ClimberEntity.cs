using UnityEngine;

using System;
using System.Threading.Tasks;

public class ClimberEntity : Entity {
    protected Collider2D collider;

    void Start() {
        collider = GetComponent<Collider2D>();
        Collider2D lHandCollider = transform.Find("LeftHand").GetComponent<Collider2D>();
        Collider2D rHandCollider = transform.Find("RightHand").GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(collider, lHandCollider);
        Physics2D.IgnoreCollision(collider, rHandCollider);
        Physics2D.IgnoreCollision(lHandCollider, rHandCollider);
    }
}