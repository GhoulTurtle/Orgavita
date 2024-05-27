using UnityEngine;

/// <summary>
/// Responsible for holding the current state of a player item, and its position and rotation. Also animates the item when changing item states.
/// </summary>
[System.Serializable]
public class PlayerItem{
    private Transform playerItemTransform;
    private Transform holsterParent;
    private Vector3 worldPostion;
    private Quaternion worldRotation;
    private PlayerItemState playerItemState;

    public void SetItemTransform(Transform itemTransform){
        playerItemTransform = itemTransform;
    }

    public void ChangeItemState(Vector3 newWorldPostion, Quaternion newWorldRotation, PlayerItemState newState, Transform parent = null){
        if(newState == playerItemState) return;

        worldPostion = newWorldPostion;
        worldRotation = newWorldRotation;
        playerItemState = newState;

        if(parent != null){
            holsterParent = parent;
            playerItemTransform.parent = holsterParent;
        }
    }
}
