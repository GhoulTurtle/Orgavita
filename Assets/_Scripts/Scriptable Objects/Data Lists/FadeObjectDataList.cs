using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data List/Fade Object Data List", fileName = "NewFadeObjectDataList")]
public class FadeObjectDataList : ScriptableObject{
    [Header("Fade Object Data")]
    [SerializeField] private List<FadeObject> fadeObjectList;
    
    public FadeObject GetFadeObject(int index){
        return fadeObjectList[index];
    }

    public FadeObject GetRandomFadeObject(){
        int randomIndex = Random.Range(0, fadeObjectList.Count);
        return fadeObjectList[randomIndex];
    }
}
