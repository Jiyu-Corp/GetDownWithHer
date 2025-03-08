using UnityEngine;
using UnityEngine.UI;

public class ClimberEntityUI : MonoBehaviour {
    [SerializeField]
    private ClimberEntity climberEntity;
    private Text staminaUI;
    private Text hpUI;

    void Start() {
        Transform canvasTransform = transform.Find("Canvas");
        
        staminaUI = canvasTransform.Find("Stamina").GetComponent<Text>();
        staminaUI.text = Mathf.FloorToInt(climberEntity.GetStamina()).ToString();
        
        hpUI = canvasTransform.Find("HP").GetComponent<Text>();
        hpUI.text = Mathf.FloorToInt(climberEntity.GetHp()).ToString();
    }

    void Update() {
        staminaUI.text = Mathf.FloorToInt(climberEntity.GetStamina()).ToString();
        hpUI.text = Mathf.FloorToInt(climberEntity.GetHp()).ToString();
    }
}