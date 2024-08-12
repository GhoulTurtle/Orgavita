using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombineUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemStackAmountText;
    [SerializeField] private TextMeshProUGUI itemNameText;

    private CombineMenuUI combineMenuUI;

    public void SetupCombineUI(CombineMenuUI _combineMenuUI){
        combineMenuUI = _combineMenuUI;
    }

    public void UpdateCombineUI(Sprite itemSprite, int stackAmount, string itemName){
        itemImage.sprite = itemSprite;
        itemNameText.text = itemName;
        itemStackAmountText.text = stackAmount.ToString();
    }
}