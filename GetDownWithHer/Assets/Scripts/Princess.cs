using UnityEngine;
using UnityEngine.SceneManagement;

public class Princess : Entity {
    [Header("Escape Settings")]
    public const float COOLDOWN_TO_ESCAPE_IN_SECONDS = 20f;

    private Transform defaultParent;

    [SerializeField]
    private Player player;

    private float currentCooldownToEscape = COOLDOWN_TO_ESCAPE_IN_SECONDS;
    private bool isCaptured = true;

    protected override void Awake() {
        base.Awake();

        defaultParent = transform.root;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if(isCaptured) TryEscape();
    }

    protected override void HealHp() {
        if (true) base.HealHp();
    }

    private void TryEscape() {
        if(player.IsAnnoyingPrincess()) return;
        
        currentCooldownToEscape -= Time.fixedDeltaTime;

        bool canEscape = currentCooldownToEscape <= 0;
        if(!canEscape) return;

        Escape();
    }

    private void Escape() {
        transform.SetParent(defaultParent);
        ManageGameObjectPhysics(true);
        
        isCaptured = false;
        player.LosePrincessReaction();
    }

    public void GetCaught() {
        ManageGameObjectPhysics(false);

        isCaptured = true;
        currentCooldownToEscape = COOLDOWN_TO_ESCAPE_IN_SECONDS;
    }

    private void ManageGameObjectPhysics(bool enable) {
        rb.simulated = enable;
        cld.enabled = enable;
    }

    public float GetCurrentCooldownToEscape() {
        return currentCooldownToEscape;
    }

    public void DelayAnnoyingly() {
        currentCooldownToEscape = COOLDOWN_TO_ESCAPE_IN_SECONDS;
    }

    protected override void Die() {
        base.Die();

        EndGame();
    }

    // We need to refactor the end/restart game logic cause the player and princess have it, code duplication... We must put it on a common place for both
    private void EndGame() {
        RestartScene();
    }
    private void RestartScene() {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
        base.OnCollisionEnter2D(collision);

        bool canKnightRescue = !isCaptured && collision.gameObject.CompareTag("Knight");
        if(canKnightRescue) EndGame();
    }
}