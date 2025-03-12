using UnityEngine;
using UnityEngine.UI;

public class ClimberEntityUI : MonoBehaviour {
    [SerializeField]
    private Player player;
    private Text staminaUI;
    private Text hpUI;
    private Text princessEscapeCooldownUI;

    void Start() {
        Transform canvasTransform = transform.Find("Canvas");
        
        staminaUI = canvasTransform.Find("Stamina").GetComponent<Text>();
        staminaUI.text = Mathf.FloorToInt(player.GetStamina()).ToString();
        
        hpUI = canvasTransform.Find("HP").GetComponent<Text>();
        hpUI.text = Mathf.FloorToInt(player.GetHp()).ToString();

        princessEscapeCooldownUI = canvasTransform.Find("PrincessEscapeCooldown").GetComponent<Text>();
        princessEscapeCooldownUI.text = Mathf.FloorToInt(player.GetCurrentCooldownToPrincessEscape()).ToString() + "s";
    }

    void Update() {
        staminaUI.text = Mathf.FloorToInt(player.GetStamina()).ToString();
        hpUI.text = Mathf.FloorToInt(player.GetHp()).ToString();
        princessEscapeCooldownUI.text = Mathf.FloorToInt(player.GetCurrentCooldownToPrincessEscape()).ToString() + "s";
        princessEscapeCooldownUI.color = player.IsAnnoyingPrincess()
            ?   Color.green
            :   Color.magenta;
    }
}