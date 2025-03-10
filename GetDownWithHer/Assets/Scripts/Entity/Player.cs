using UnityEngine;

public class Player : ClimberEntity {
    private bool havePrincess = true;

    private void LoseGame() {
        RestartScene();
    }
    private void RestartScene() {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void CatchPrincess(GameObject princessObj) {
        Princess princessScript = princessObj.GetComponent<Princess>();
        princessObj.transform.SetParent(transform);

        // Temporary princess catch design
        Vector2 parentSize = cld.bounds.size;
        princessObj.transform.localPosition = new Vector2(-parentSize.x/2 + 2, parentSize.y/2 - 2);
        princessObj.transform.Rotate(0f, 0f, 45f);

        havePrincess = true;
        princessScript.GetCaught();
    }

    public void LosePrincessReaction() {
        havePrincess = false;
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
        base.OnCollisionEnter2D(collision);

        bool knightHit = collision.gameObject.CompareTag("Knight");
        if(knightHit) LoseGame();

        bool canCatchPrincess = !havePrincess && collision.gameObject.CompareTag("Princess");
        if(canCatchPrincess) CatchPrincess(collision.gameObject);
    }
}