using UnityEngine;

public class Princess : Entity {
    public const float MAX_HP = 50f;

    [Header("Escape Settings")]
    public const float COOLDOWN_TO_ESCAPE_IN_SECONDS = 120f;

    private readonly Transform DEFAULT_PARENT = transform.root;

    [SerializeField]
    private Player player;

    private float currentCooldownToEscape = COOLDOWN_TO_ESCAPE_IN_SECONDS;
    private bool isCaptured = true;

    protected override void FixedUpdate() {
        if(isCaptured) TryEscape();
    }

    protected override void HealHp() {};

    private void TryEscape() {
        currentCooldownToEscape -= Time.fixedDeltaTime;

        bool canEscape = currentCooldownToEscape <= 0;
        if(!canEscape) return;

        Escape();
    }

    private void Escape() {
        transform.localPosition = new Vector2(transform.localPosition.x-2, transform.localPosition.y);

        transform.SetParent(DEFAULT_PARENT);
        ManageGameObjectPhysics(true);
        
        isCaptured = false;
        player.LosePrincessReaction();
    }

    public void GetCaught() {
        ManageGameObjectPhysics(false);

        isCaptured = true;
        currentCooldownToEscape = COOLDOWN_TO_ESCAPE_IN_SECONDS;
    }

    private void ManageGameObjectPhysics(enable) {
        rb.simulated = enable;
        cld.enabled = enable;
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