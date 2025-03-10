using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : ClimberEntity {
    [Header("Catch Princess Settings")]
    public const float COOLDOWN_TO_CATCH_PRINCESS = 5f;

    private bool isCatchPrincessOnCooldown = false;
    private float currentCooldownToCatchPrincess = COOLDOWN_TO_CATCH_PRINCESS;
    private bool havePrincess = true;

    [SerializeField]
    protected Collider2D princessCollider;

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if(isCatchPrincessOnCooldown) ManageCatchPrincessCooldown();
    }

    private void ManageCatchPrincessCooldown() {
        currentCooldownToCatchPrincess -= Time.fixedDeltaTime;

        if(currentCooldownToCatchPrincess <= 0) {
            isCatchPrincessOnCooldown = false;

            Physics2D.IgnoreCollision(lHandCollider, princessCollider, false);
            Physics2D.IgnoreCollision(rHandCollider, princessCollider, false);
            Physics2D.IgnoreCollision(cld, princessCollider, false);
        }
    }

    private void CatchPrincess(GameObject princessObj) {
        Princess princessScript = princessObj.GetComponent<Princess>();
        princessObj.transform.SetParent(transform);

        // Temporary princess catch design
        princessObj.transform.localPosition = new Vector2(-0.41f, 0.42f);
        princessObj.transform.localRotation = Quaternion.Euler(0f, 0f, 123.33f);

        havePrincess = true;
        princessScript.GetCaught();
    }

    public void LosePrincessReaction() {
        isCatchPrincessOnCooldown = true;
        currentCooldownToCatchPrincess = COOLDOWN_TO_CATCH_PRINCESS;
        havePrincess = false;

        Physics2D.IgnoreCollision(lHandCollider, princessCollider);
        Physics2D.IgnoreCollision(rHandCollider, princessCollider);
        Physics2D.IgnoreCollision(cld, princessCollider);
    }

    protected override void Die() {
        base.Die();

        LoseGame();
    }

    private void LoseGame() {
        RestartScene();
    }
    private void RestartScene() {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
        base.OnCollisionEnter2D(collision);

        bool knightHit = collision.gameObject.CompareTag("Knight");
        if(knightHit) LoseGame();

        bool canCatchPrincess = !havePrincess && collision.gameObject.CompareTag("Princess");
        if(canCatchPrincess) CatchPrincess(collision.gameObject);
    }
}