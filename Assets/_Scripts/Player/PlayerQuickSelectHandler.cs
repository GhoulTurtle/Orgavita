using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerQuickSelectHandler : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private QuickSelectDataSO quickSelectDataSO; 
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private PlayerEquippedItemHandler playerEquippedItemHandler;

    [Header("Quick Selection Variables")]
    [SerializeField] private float quickSelectInputCooldownInSeconds = 0.25f;

    [Header("Quick Selection Popup Variables")]
    [SerializeField] private float popupPrintSpeed = 0.01f;
    [SerializeField] private float popupWaitTime = 1.5f;
    [SerializeField] private float popupFadeTime = 1f;

    private IEnumerator quickSelectInputCooldownCoroutine;

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void AttemptQuickSelectAssignment(int slotIndex){
        ItemDataSO currentSelectedItem = inventoryUI.GetSelectedItemData();
        
        quickSelectDataSO.AttemptAssignItem(currentSelectedItem, slotIndex);

        playerInventoryHandler.UpdateInventoryState(InventoryState.ContextUI);
    }

    public QuickSelectDataSO GetQuickSelectDataSO(){
        return quickSelectDataSO;
    }

    public void QuickSelectInput(InputAction.CallbackContext context){
        if(quickSelectInputCooldownCoroutine != null) return;

        if(context.phase != InputActionPhase.Performed) return;

        Vector2 input = context.ReadValue<Vector2>().normalized;

        if(input == Vector2.up){
            QuickSelect(0);
            return;
        }
        
        if(input == Vector2.right){
            QuickSelect(1);
            return;
        }

        if(input == Vector2.down){
            QuickSelect(2);
            return;
        }

        if(input == Vector2.left){
            QuickSelect(3);
            return;
        }
    }
    
    private void QuickSelect(int slotIndex){
        ItemDataSO itemData = quickSelectDataSO.GetItemDataSO(slotIndex);
        if(itemData == null) return;

        Dialogue resultDialogue = new Dialogue();

        if(!playerInventoryHandler.GetInventory().HasItemInInventory(itemData)){
            resultDialogue.sentence = "<color=\"red\">No " + itemData.GetItemName() + " in inventory.</color>";
            PopupUI.Instance.PrintText(resultDialogue, popupPrintSpeed, true, popupWaitTime, popupFadeTime);
            return;
        }

        InventoryItem inventoryItem = playerInventoryHandler.GetInventory().AttemptGetInventoryItem(itemData);

        switch (itemData.GetItemType()){
            case ItemType.Key_Item or ItemType.Consumable: 
                itemData.UseItem(inventoryItem, playerInventoryHandler, out resultDialogue.sentence);
                quickSelectInputCooldownCoroutine = QuickSelectInputCooldownCoroutine();
                StartCoroutine(quickSelectInputCooldownCoroutine);
                break;
            case ItemType.Weapon:
                //See if the item is active
                if(playerEquippedItemHandler.IsWeaponItemEquipped(itemData)){
                    if(playerEquippedItemHandler.IsCurrentWeaponActive()){
                        resultDialogue.sentence = itemData.GetItemName() + " is equipped.";
                    }
                    else{
                        playerEquippedItemHandler.UnholsterActiveWeapon();
                        resultDialogue.sentence =  "Unholstered " + itemData.GetItemName();
                        quickSelectInputCooldownCoroutine = QuickSelectInputCooldownCoroutine();
                        StartCoroutine(quickSelectInputCooldownCoroutine);
                    }
                }
                else{
                    if(playerEquippedItemHandler.IsCurrentWeaponActive()){
                        //Check if we can swap off the current weapon
                        GunWeaponEquippedItemBehaviour currentWeapon = (GunWeaponEquippedItemBehaviour)playerEquippedItemHandler.GetCurrentWeaponItemBehaviour();

                        if(!currentWeapon.CanSwitchFromWeapon()){
                            resultDialogue.sentence = "<color=\"red\">Can't switch right now</color>";
                            PopupUI.Instance.PrintText(resultDialogue, popupPrintSpeed, true, popupWaitTime, popupFadeTime);
                            return;
                        }
                    }

                    playerInventoryHandler.GetInventory().EquipWeaponItem(inventoryItem);
                    resultDialogue.sentence = "Equipped " + itemData.GetItemName();
                    quickSelectInputCooldownCoroutine = QuickSelectInputCooldownCoroutine();
                    StartCoroutine(quickSelectInputCooldownCoroutine);
                }
                break;
            case ItemType.Tool:
                //See if the item is active
                if(playerEquippedItemHandler.IsToolItemEquipped(itemData)){
                    if(playerEquippedItemHandler.IsCurrentToolActive()){
                        resultDialogue.sentence = itemData.GetItemName() + " is equipped.";
                    }
                    else{
                        playerEquippedItemHandler.UnholsterActiveTool();
                        resultDialogue.sentence =  "Unholstered " + itemData.GetItemName();
                        quickSelectInputCooldownCoroutine = QuickSelectInputCooldownCoroutine();
                        StartCoroutine(quickSelectInputCooldownCoroutine);
                    }
                }
                else{
                    playerInventoryHandler.GetInventory().EquipToolItem(inventoryItem);
                    resultDialogue.sentence = "Equipped " + itemData.GetItemName();
                    quickSelectInputCooldownCoroutine = QuickSelectInputCooldownCoroutine();
                    StartCoroutine(quickSelectInputCooldownCoroutine);
                }
                break;
        }

        //Check if the item was a one use key item
        if(itemData.GetItemType() == ItemType.Key_Item && !playerInventoryHandler.GetInventory().HasItemInInventory(itemData)){
            quickSelectDataSO.RemoveItemData(itemData);
        }

        PopupUI.Instance.PrintText(resultDialogue, popupPrintSpeed, true, popupWaitTime, popupFadeTime);
    }

    private IEnumerator QuickSelectInputCooldownCoroutine(){
        yield return new WaitForSeconds(quickSelectInputCooldownInSeconds);

        quickSelectInputCooldownCoroutine = null;
    }
}