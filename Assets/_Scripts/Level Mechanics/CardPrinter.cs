using UnityEngine;

public class CardPrinter : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private CardPrinterResourceDataSO resourceDataSO;
    [SerializeField] private ItemLevelMechanic itemLevelMechanic;
    [SerializeField] private ItemPickup itemEjectPickup;

    private void Awake() {
        if(itemLevelMechanic != null){
            itemLevelMechanic.OnUnlockAction += UpdateCardPrinterResouceData;
        }
    }

    private void OnDestroy() {
        if(itemLevelMechanic != null){
            itemLevelMechanic.OnUnlockAction -= UpdateCardPrinterResouceData;
        }
    }

    public void EjectHeldItem(){
        if(!resourceDataSO.IsHoldingItem()) return;

        ItemDataSO itemDataSO = resourceDataSO.GetCurrentHeldItem();

        itemEjectPickup.SetItemPickup(itemDataSO, 1);

        resourceDataSO.RemoveItem();

        itemLevelMechanic.SetIsUnlocked(false);
    }

    public CardPrinterResourceDataSO GetCardPrinterResourceData(){
        return resourceDataSO;
    }

    private void UpdateCardPrinterResouceData(ItemDataSO itemData){
        resourceDataSO.HoldNewItem(itemData);
    }
}
