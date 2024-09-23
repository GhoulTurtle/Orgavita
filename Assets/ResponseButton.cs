using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResponseButton : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private Button button;

    public float GetButtonSizeYDelta(){
        return rectTransform.sizeDelta.y;
    }

    public void SetButtonText(string text){
        textMeshProUGUI.text = text;
    }

    public void SetButtonListener(UnityAction call){
        button.onClick.AddListener(call);
    }

    public void RemoveAllButtonListeners(){
        button.onClick.RemoveAllListeners();
    }
}
