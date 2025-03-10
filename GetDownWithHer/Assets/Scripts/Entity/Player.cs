using UnityEngine;

public class Player : ClimberEntity {
    private void LoseGame() {
        RestartScene();
    }
    private void RestartScene() {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        base.OnCollisionEnter2D(collision);

        bool knightHit = collision.gameObject.CompareTag("Knight");
        if(knightHit) LoseGame();
    }
}