using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : ClimberEntity {
    [Header("Catch Princess Settings")]
    public const float COOLDOWN_TO_CATCH_PRINCESS = 5f;

    private bool isCatchPrincessOnCooldown = false;
    private float currentCooldownToCatchPrincess = COOLDOWN_TO_CATCH_PRINCESS;
    private bool havePrincess = true;

    [Header("Annoy Princess Settings")]
    public const float TIME_NEEDED_TO_DELAY_PRINCESS_WITH_ANNOY_IN_SECONDS = 4f;

    private float startAnnoyPrincessCallTime = 0f;

    [SerializeField]
    protected Collider2D princessCollider;
    [SerializeField]
    protected Princess princessScript;

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if(isCatchPrincessOnCooldown) ManageCatchPrincessCooldown();
        if(startAnnoyPrincessCallTime != 0) AnnoyPrincess();
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

    public bool IsAnnoyingPrincess() {
        bool isAnnoyingPrincess = startAnnoyPrincessCallTime != 0;
        return isAnnoyingPrincess;
    }

    private void CatchPrincess(GameObject princessObj) {
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

    private void AnnoyPrincess() {
        if(Mathf.Abs(rb.linearVelocity.x) > 1 || Mathf.Abs(rb.linearVelocity.y) > 1) StopAnnoyPrincess();

        if(Time.time - startAnnoyPrincessCallTime >= TIME_NEEDED_TO_DELAY_PRINCESS_WITH_ANNOY_IN_SECONDS) princessScript.DelayAnnoyingly();
    }

    public void StartAnnoyPrincess() {
        if(!inGround) return;

        startAnnoyPrincessCallTime = Time.time;
    }

    public void StopAnnoyPrincess() {
        startAnnoyPrincessCallTime = 0f;
    }

    public float GetCurrentCooldownToPrincessEscape() {
        return princessScript.GetCurrentCooldownToEscape();
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