using System;
using UnityEngine.Events;

public class ContextButton{
    private ContextMenuUI contextMenuUI;
    private ContextButtonType buttonType;

    private PlayerInventoryHandler playerInventoryHandler;
    private PlayerInventorySO playerInventory;
    private InventoryItem selectedInventoryItem;

    private bool isConfirmingDestroy = false;

    public ContextButton(ContextButtonType _buttonType, ContextMenuUI _contextMenuUI, InventoryItem _selectedInventoryItem, PlayerInventoryHandler _playerInventoryHandler){
        buttonType = _buttonType;
        contextMenuUI = _contextMenuUI;
        selectedInventoryItem = _selectedInventoryItem;
        playerInventoryHandler = _playerInventoryHandler;

        playerInventory = _playerInventoryHandler.GetInventory();
    }

    public ContextButtonType GetContextButtonType(){
        return buttonType;
    }

    public UnityAction GetButtonAction(){
        return buttonType switch{
            ContextButtonType.Use => UseAction,
            ContextButtonType.Equip => EquipAction,
            ContextButtonType.UnEquip => UnEquipAction,
            ContextButtonType.Inspect => InspectAction,
            ContextButtonType.Assign => AssignAction,
            ContextButtonType.Combine => CombineAction,
            ContextButtonType.Move => MoveAction,
            ContextButtonType.Destroy => DestroyAction,
            _ => null,
        };
    }

    private void UseAction(){
        selectedInventoryItem.GetHeldItem().UseItem(selectedInventoryItem, playerInventoryHandler, out string resultMessage);
        playerInventoryHandler.UpdateInventoryState(InventoryState.Default);
        contextMenuUI.ShowUseResultText(resultMessage);
    }

    private void InspectAction(){
        playerInventoryHandler.UpdateInventoryState(InventoryState.Inspect);
    }

    private void AssignAction(){
        playerInventoryHandler.UpdateInventoryState(InventoryState.Assign);
    }

    private void CombineAction(){
        playerInventoryHandler.UpdateInventoryState(InventoryState.Combine);
    }

    private void MoveAction(){
        playerInventoryHandler.UpdateInventoryState(InventoryState.Move);
    }

    private void DestroyAction(){
        if(isConfirmingDestroy){
            selectedInventoryItem.ClearItem();
            playerInventoryHandler.UpdateInventoryState(InventoryState.Default);
            return;
        }

        contextMenuUI.DestroyConfirmationText();
        isConfirmingDestroy = true;
    }

    private void UnEquipAction(){
        if(selectedInventoryItem.GetHeldItem().GetItemType() == ItemType.Weapon){
            playerInventory.UnEquipWeaponItem();
        }
        else if(selectedInventoryItem.GetHeldItem().GetItemType() == ItemType.Tool){
            playerInventory.UnEquipEmergencyItem();
        }

        playerInventoryHandler.UpdateInventoryState(InventoryState.Default);
    }

    private void EquipAction(){
        if(selectedInventoryItem.GetHeldItem().GetItemType() == ItemType.Weapon){
            playerInventory.EquipWeaponItem(selectedInventoryItem);
        }
        else if(selectedInventoryItem.GetHeldItem().GetItemType() == ItemType.Tool){
            playerInventory.EquipEmergencyItem(selectedInventoryItem);
        }

        playerInventoryHandler.UpdateInventoryState(InventoryState.Default);
    }
}