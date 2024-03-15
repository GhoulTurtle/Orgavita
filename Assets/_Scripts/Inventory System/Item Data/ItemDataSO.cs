using UnityEngine;

/// <summary>
/// Base scriptable object that all items derive from. Holds needed variables and methods for items.
/// </summary>
public abstract class ItemDataSO : ScriptableObject{
    [Header("Item Information")]
    public string ItemName;
    public string ItemQuickDescription;
    public BasicDialogueSO ItemInspectDescription;
    
    [Header("Item Visual")]
    public Sprite ItemIcon;
    public Transform ItemModelPrefab;

    [Header("Item Settings")]
    public bool IsCombinable = false;
    public bool IsStackable = true;
    public bool IsDestroyable = true;
    public int maxStackSize;

    public abstract bool UseItem();
    public abstract void CombineItem();
    public abstract void DestoryItem();
}