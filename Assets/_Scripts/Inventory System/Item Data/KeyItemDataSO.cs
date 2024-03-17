using UnityEngine;

[CreateAssetMenu(menuName = "Item/Key Item", fileName = "NewKeyItemDataSO")]
public class KeyItemDataSO : ItemDataSO{
    public override bool UseItem(){
        Debug.Log("Used KeyItem");
        return true;
    }
}
