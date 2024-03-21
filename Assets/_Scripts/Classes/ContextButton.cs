using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            ContextButtonType.Combine => CombineAction,
            ContextButtonType.Destroy => DestroyAction,
            _ => null,
        };
    }

    private void UseAction(){
        selectedInventoryItem.GetHeldItem().UseItem();
        playerInventoryHandler.UpdateInventoryState(InventoryState.Default);
    }

    private void InspectAction(){
        playerInventoryHandler.UpdateInventoryState(InventoryState.Inspect);
    }

    private void CombineAction(){
        playerInventoryHandler.UpdateInventoryState(InventoryState.Combine);
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
            playerInventory.UnEquipItem();
            return;
        }

        if(selectedInventoryItem.GetHeldItem().GetItemType() == ItemType.Emergency_Item){
            playerInventory.UnEquipEmergencyItem();
            return;
        }
    }

    private void EquipAction(){
        if(selectedInventoryItem.GetHeldItem().GetItemType() == ItemType.Weapon){
            playerInventory.EquipItem(selectedInventoryItem);
            return;
        }

        if(selectedInventoryItem.GetHeldItem().GetItemType() == ItemType.Emergency_Item){
            playerInventory.EquipEmergencyItem(selectedInventoryItem);
            return;
        }
    }
}
