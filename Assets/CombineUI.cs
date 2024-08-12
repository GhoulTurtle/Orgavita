using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombineUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemStackAmount;

    private CombineMenuUI combineMenuUI;

    public void SetupCombineUI(CombineMenuUI _combineMenuUI){
        combineMenuUI = _combineMenuUI;
    }

    public void UpdateCombineUI(Sprite itemSprite, int stackAmount){
        itemImage.sprite = itemSprite;
        itemStackAmount.text = stackAmount.ToString();
    }
}