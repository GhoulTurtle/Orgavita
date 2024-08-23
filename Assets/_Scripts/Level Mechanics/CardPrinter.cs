using UnityEngine;

public class CardPrinter : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private CardPrinterResourceDataSO resourceDataSO;
    [SerializeField] private ItemLevelMechanic itemLevelMechanic;
    [SerializeField] private ItemPickup itemEjectPickup;
    [SerializeField] private ComputerUI connectedComputerUI;
    [SerializeField] private ComputerApplication correctComputerApplication;

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

        if(cardPrinterApplicationUI != null){
            cardPrinterApplicationUI.OnCreateNewCard -= CreateNewCard;
            cardPrinterApplicationUI.OnUpgradeCard -= UpgradeCard;
            cardPrinterApplicationUI.OnEjectCard -= EjectCard;
        }

        
        if(resourceDataSO != null){
            resourceDataSO.OnResourceUpdated -= UpdateCardPrinterStatus;
        }
    }

    public void EjectHeldItem(){
        if(!resourceDataSO.IsHoldingItem()) return;

        ItemDataSO itemDataSO = resourceDataSO.GetCurrentHeldItem();

        itemEjectPickup.SetItemPickup(itemDataSO, 1);

        resourceDataSO.RemoveItem();

        itemLevelMechanic.SetIsUnlocked(false);

        UpdateCardPrinterStatus(0);
    }

    public CardPrinterResourceDataSO GetCardPrinterResourceData(){
        return resourceDataSO;
    }

    public CardPrinterStatus GetCardPrinterStatus(){
        return cardPrinterStatus;
    }

    private void EvaulateApplication(ComputerApplication application, ApplicationUI applicationUIInstance){
        if(application == correctComputerApplication){
            cardPrinterApplicationUI = (CardPrinterApplicationUI)applicationUIInstance;
            cardPrinterApplicationUI.OnCreateNewCard += CreateNewCard;
            cardPrinterApplicationUI.OnUpgradeCard += UpgradeCard;
            cardPrinterApplicationUI.OnEjectCard += EjectCard;

            cardPrinterApplicationUI.SetupCardPrinterUI(this);

            cardPrinterApplicationUI.UpdateCardPrinterStatusUI(cardPrinterStatus);
        }    
    }

    private void EvaulateApplicationClosed(ComputerApplication application){
        if(application == correctComputerApplication && cardPrinterApplicationUI != null){
            cardPrinterApplicationUI.OnCreateNewCard -= CreateNewCard;
            cardPrinterApplicationUI.OnUpgradeCard -= UpgradeCard;
            cardPrinterApplicationUI.OnEjectCard -= EjectCard;
            cardPrinterApplicationUI = null;
        }    
    }
    
    private void UpdateCardPrinterStatus(int obj){
        if(!resourceDataSO.IsHoldingItem()){
            cardPrinterStatus = CardPrinterStatus.Invalid_Print_No_Card;
        }   
        else if(!resourceDataSO.IsHeldItemCorrect()){
            cardPrinterStatus = CardPrinterStatus.Invalid_Print_Wrong_Card;
        }
        else{
            cardPrinterStatus = CardPrinterStatus.Valid_Print;
        }

        if(cardPrinterApplicationUI != null){
            cardPrinterApplicationUI.UpdateCardPrinterStatusUI(cardPrinterStatus);
        }
    }

    private void CreateNewCard(){
        Debug.Log("Creating a new card...");
    }

    private void UpgradeCard(){
        Debug.Log("Upgrading card...");
    }

    private void EjectCard(){
        Debug.Log("Ejecting card...");
        EjectHeldItem();
    }

    private void UpdateCardPrinterResouceData(ItemDataSO itemData){
        resourceDataSO.HoldNewItem(itemData);
    }
}
