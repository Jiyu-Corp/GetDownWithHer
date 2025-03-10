using UnityEngine;
using UnityEngine.UI;

public class ClimberEntityUI : MonoBehaviour {
    [SerializeField]
    private Player player;
    private Text staminaUI;
    private Text hpUI;

    void Start() {
        Transform canvasTransform = transform.Find("Canvas");
        
        staminaUI = canvasTransform.Find("Stamina").GetComponent<Text>();
        staminaUI.text = Mathf.FloorToInt(player.GetStamina()).ToString();
        
        hpUI = canvasTransform.Find("HP").GetComponent<Text>();
        hpUI.text = Mathf.FloorToInt(player.GetHp()).ToString();
    }

    void Update() {
        staminaUI.text = Mathf.FloorToInt(player.GetStamina()).ToString();
        hpUI.text = Mathf.FloorToInt(player.GetHp()).ToString();
    }
}