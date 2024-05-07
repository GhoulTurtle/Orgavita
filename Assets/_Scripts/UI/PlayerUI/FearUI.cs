using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FearUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private Fear playerFear;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image statusImage;

    [Header("FearUI Variables")]
    [SerializeField] private Color calmColor = Color.green;
    [SerializeField] private Color panicColor = Color.magenta;
    [SerializeField] private Color terrifiedColor = Color.white;

    private void Awake() {
        playerFear.OnFearStateChanged += UpdateFearUI;
    }

    private void OnDestroy() {
        playerFear.OnFearStateChanged -= UpdateFearUI;        
    }

    private void UpdateFearUI(object sender, Fear.FearStateChangedEventArgs e){        
        switch (e.fearState){
        case FearState.Calm:
            statusText.text = "CALM";
            statusImage.color = calmColor;
            break;
        case FearState.Panic:
            statusText.text = "PANIC";
            statusImage.color = panicColor;
            break;
        case FearState.Terrified:
            statusText.text = "TERRIFIED";
            statusImage.color = terrifiedColor;
            break;
        }
    }
}
