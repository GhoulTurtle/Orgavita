using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data/Quick Select Data", fileName = "PlayerQuickSelectDataSO")]
public class QuickSelectDataSO : ScriptableObject{
    [Header("Quick Select Slots")]
    public List<ItemDataSO> quickSelectItemDataList = new();

    [Header("Debug Variables")]
    public bool clearSelectionDataOnStart;

    public Action OnQuickSelectUpdated;

#if UNITY_EDITOR
    private void OnEnable() {
        if (clearSelectionDataOnStart){
            ClearQuickSelectData();
        }
    }
#endif

    public void AttemptAssignItem(ItemDataSO itemDataSO, int slotNum){
        //Check if the item is already in another slot
        if(IsItemDataInExistingSlot(itemDataSO, out int index)){
            if(index == slotNum){
                quickSelectItemDataList[slotNum] = null;
                OnQuickSelectUpdated?.Invoke();
                return;
            }

            quickSelectItemDataList[index] = null;
        }

        quickSelectItemDataList[slotNum] = itemDataSO;
        OnQuickSelectUpdated?.Invoke();
    }

    public void RemoveItemData(ItemDataSO itemDataSO){
        for (int i = 0; i < quickSelectItemDataList.Count; i++){
            if(quickSelectItemDataList[i] == itemDataSO){
                quickSelectItemDataList[i] = null;
                OnQuickSelectUpdated?.Invoke();
                return;
            }
        }
    }

    public ItemDataSO GetItemDataSO(int slotNum){
        if(slotNum < 0 || slotNum >= quickSelectItemDataList.Count) return null;
        return quickSelectItemDataList[slotNum];
    }

    private bool IsItemDataInExistingSlot(ItemDataSO itemDataSO, out int index){
        if(quickSelectItemDataList.Contains(itemDataSO)){
            index = quickSelectItemDataList.IndexOf(itemDataSO);
            return true;
        }

        index = -1;
        return false;
    }

    public void ClearQuickSelectData(){
        for (int i = 0; i < quickSelectItemDataList.Count; i++){
            quickSelectItemDataList[i] = null;
        }
    }
}
