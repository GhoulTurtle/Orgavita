using System.Collections.Generic;
using UnityEngine;

public class CardPrinter : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private CardPrinterResourceDataSO resourceDataSO;
    [SerializeField] private ItemLevelMechanic itemLevelMechanic;
    [SerializeField] private ItemPickup itemEjectPickup;
    [SerializeField] private ComputerUI connectedComputerUI;
    [SerializeField] private ComputerApplication correctComputerApplication;
    [SerializeField] private List<ItemDataSO> upgradeCardItemDataList;
    
    //Going to need to change this for different cards but this will work for now
    [SerializeField] private ItemDataSO createCardItemData;
    [SerializeField] private IntegarCodeSO correctNewCardCode;
    [SerializeField] private List<IntegarCodeSO> inUseCardCodeList = new();

    [Header("Card Printer Variables")]
    [SerializeField] private bool canUpgradeCards = false;

    private CardPrinterApplicationUI cardPrinterApplicationUI;

    private CardPrinterStatus cardPrinterStatus = CardPrinterStatus.Invalid_Print_No_Card;

    private void Awake() {
        if(itemLevelMechanic != null){
            itemLevelMechanic.OnUnlockAction += UpdateCardPrinterResouceData;
        }

        if(connectedComputerUI != null){
            connectedComputerUI.OnApplicationSetup += EvaulateApplication;
            connectedComputerUI.OnApplicationClosed += EvaulateApplicationClosed;
        }

        if(resourceDataSO != null){
            resourceDataSO.OnResourceUpdated += UpdateCardPrinterStatus;
        }
    }

    private void OnDestroy() {
        if(itemLevelMechanic != null){
            itemLevelMechanic.OnUnlockAction -= UpdateCardPrinterResouceData;
        }

        if(connectedComputerUI != null){
            connectedComputerUI.OnApplicationSetup -= EvaulateApplication;
            connectedComputerUI.OnApplicationClosed -= EvaulateApplicationClosed;
        }
        
        if(resourceDataSO != null){
            resourceDataSO.OnResourceUpdated -= UpdateCardPrinterStatus;
        }
    }

    public void EjectHeldItem(){
        if(!resourceDataSO.IsHoldingItem()) return;

        ItemDataSO itemDataSO = resourceDataSO.GetCurrentHeldItem();

        itemEjectPickup.SetItemPickup(itemDataSO, resourceDataSO.GetCurrentStackCount());

        resourceDataSO.ClearItem();

        itemLevelMechanic.SetIsUnlocked(false);

        UpdateCardPrinterStatus(0);
    }

    public bool CanUpgradeCard(){
        if(!resourceDataSO.IsHoldingItem()) return false;

        ItemDataSO itemDataSO = resourceDataSO.GetCurrentHeldItem();

        if(!upgradeCardItemDataList.Contains(itemDataSO)) return false;

        int cardLevelIndex = upgradeCardItemDataList.IndexOf(itemDataSO);
        
        if(cardLevelIndex == upgradeCardItemDataList.Count - 1) return false;
        
        return true;
    }

    public void CreateNewCard(){
        resourceDataSO.UpdateHeldItem(createCardItemData);
    }

    public CardPrinterSearchResultType AttemptCreateNewCard(string codeInputted){
        if(correctNewCardCode.IsCodeCorrect(codeInputted)){
            return CardPrinterSearchResultType.Valid_Code;
        }

        if(inUseCardCodeList.Count == 0) return CardPrinterSearchResultType.Invalid_Code;
        
        for (int i = 0; i < inUseCardCodeList.Count; i++){
            if(inUseCardCodeList[i].IsCodeCorrect(codeInputted)){
                return CardPrinterSearchResultType.Code_In_Use;
            }
        }

        return CardPrinterSearchResultType.Invalid_Code;
    }

    public CardPrinterResourceDataSO GetCardPrinterResourceData(){
        return resourceDataSO;
    }

    public CardPrinterStatus GetCardPrinterStatus(){
        return cardPrinterStatus;
    }

    public bool CanUpgradeCards(){
        return canUpgradeCards;
    }

    public bool CanCreateNewCard(){
        if(!resourceDataSO.IsHoldingItem()) return false;

        return resourceDataSO.IsHeldItemCorrect();
    }

    private void EvaulateApplication(ComputerApplication application, ApplicationUI applicationUIInstance){
        if(application == correctComputerApplication){
            cardPrinterApplicationUI = (CardPrinterApplicationUI)applicationUIInstance;

            cardPrinterApplicationUI.SetupCardPrinterUI(this);

            cardPrinterApplicationUI.UpdateCardPrinterStatusUI(cardPrinterStatus);
        }    
    }

    private void EvaulateApplicationClosed(ComputerApplication application){
        if(application == correctComputerApplication && cardPrinterApplicationUI != null){
            cardPrinterApplicationUI = null;
        }    
    }
    
    private void UpdateCardPrinterStatus(int obj){
        if(!resourceDataSO.IsHoldingItem()){
            cardPrinterStatus = CardPrinterStatus.Invalid_Print_No_Card;
        }   
        else if(!resourceDataSO.IsHeldItemCorrect() && resourceDataSO.GetCurrentHeldItem() != createCardItemData){
            cardPrinterStatus = CardPrinterStatus.Invalid_Print_Wrong_Card;
        }
        else{
            cardPrinterStatus = CardPrinterStatus.Valid_Print;
        }

        if(cardPrinterApplicationUI != null){
            cardPrinterApplicationUI.UpdateCardPrinterStatusUI(cardPrinterStatus);
        }
    }

    private void UpdateCardPrinterResouceData(ItemDataSO itemData){
        resourceDataSO.HoldNewItem(itemData);
    }
}
