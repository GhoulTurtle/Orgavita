using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CrematoriumPressZone{
    public Image pressZoneImage; 
    [Range(0, 3)] public int associatedSliderIndex;
    public float minPressAmount;
    public float maxPressAmount;

    public bool IsPressValid(float amount){
        if(amount < minPressAmount) return false;
        if(amount > maxPressAmount) return false;

        return true;
    }
}
