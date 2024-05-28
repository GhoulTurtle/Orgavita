using System;
using UnityEngine;

/// <summary>
/// Responsible for holding the current state of a player item, and its position and rotation. Also animates the item when changing item states.
/// </summary>
/// 
[Serializable]
public class PlayerItem{
    private Transform playerItemTransform;
    private Transform currentHolsterParent;
    private Transform defaultHolsterParent;

    [Header("Item State Variables")]
    public PlayerItemState PlayerItemState;
    public PlayerItemHolsterType PlayerItemHolsterType;

    public EventHandler<ItemStateChangedEventArgs> OnItemStateChanged;

    public class ItemStateChangedEventArgs : EventArgs{
        public PlayerItemState incomingState;

        public ItemStateChangedEventArgs(PlayerItemState _incomingeState){
            incomingState = _incomingeState;
        }
    }

    public PlayerItem(Transform _playerItemTransform){
        playerItemTransform = _playerItemTransform;
    }

    public void SetupPlayerItemHolster(Transform _defaultHolsterParent){
        defaultHolsterParent = _defaultHolsterParent;
    }

    public void TriggerDefaultState(){
        OnItemStateChanged?.Invoke(this, new ItemStateChangedEventArgs(PlayerItemState));
        ChangeItemHolster(defaultHolsterParent);

        //TO-DO: Set postion and rotation.
    }

    public void ChangeItemState(PlayerItemState newState, Transform positionParent = null){
        if(newState == PlayerItemState) return;

        PlayerItemState = newState;

        if(positionParent != null){
            ChangeItemHolster(positionParent);
        }

        OnItemStateChanged?.Invoke(this, new ItemStateChangedEventArgs(newState));
    }

    public PlayerItemHolsterType GetPlayerItemHolsterType(){
        return PlayerItemHolsterType;
    }

    public PlayerItemState GetPlayerItemState(){
        return PlayerItemState;      
    }

    private void ChangeItemHolster(Transform holsterTransform){
        currentHolsterParent = holsterTransform;
        playerItemTransform.parent = currentHolsterParent;
    }
}