using System;
using UnityEngine;

/// <summary>
/// The base scriptable object that holds resource data for a equippable item behaviour
/// </summary>
[CreateAssetMenu(menuName = "Resource Data/Basic Resource Data", fileName = "NewResourceDataSO")]
public class ResourceDataSO : ScriptableObject{
    [Header("Resource Data Variables")]
    [SerializeField] protected ItemDataSO itemDataToHold;
    [SerializeField] protected int currentStack;    
    [SerializeField] protected int maxStack;

#if UNITY_EDITOR
    [Header("Unity Editor Variables")]
    [SerializeField] protected bool resetResourceStack;
    [SerializeField] protected int defaultStack;
#endif
    
#if UNITY_EDITOR
    public void OnEnable(){
        if(resetResourceStack){ 
            currentStack = defaultStack;
        }
    }
#endif

    public Action<int> OnResourceUpdated;
    public Action OnMaxStackUpdated;

    public virtual void AddItemStack(int stackAmount){
        currentStack += stackAmount;
        if(currentStack > maxStack){
            currentStack = maxStack;
        }

        OnResourceUpdated?.Invoke(currentStack);
    }

    public virtual void RemoveItem(){
        if(!IsEmpty()){
            currentStack--;
            OnResourceUpdated?.Invoke(currentStack);
        }
    }

    public virtual void IncreaseMaxStack(int amount){
        maxStack += amount;
        OnMaxStackUpdated?.Invoke();
    }

    public virtual bool IsEmpty(){
        return currentStack == 0;
    }

    public virtual bool IsFull(){
        return currentStack == maxStack;
    }

    public virtual ItemDataSO GetValidItemData(){
        return itemDataToHold;
    }

    public virtual int GetCurrentStackCount(){
        return currentStack;
    }

    public virtual int GetMaxStackCount(){
        return maxStack;
    }

    public virtual int GetMissingStackCount(){
        return maxStack - currentStack;
    }
}