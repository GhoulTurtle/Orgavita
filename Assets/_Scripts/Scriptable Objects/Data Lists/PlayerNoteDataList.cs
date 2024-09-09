using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data List/Player Note Data List", fileName = "NewPlayerNoteDataList")]
public class PlayerNoteDataList : ScriptableObject{
    public List<NoteSO> noteDataList = new List<NoteSO>();

    [Header("Debugging")]
    public bool setDataListToDefaultOnStart = false;
    public List<NoteSO> defaultNoteDataList = new List<NoteSO>();

    public void OnEnable(){
        if(setDataListToDefaultOnStart){
            noteDataList.Clear();
            noteDataList = defaultNoteDataList;
        }
    }

    public void AddNewNoteToDataList(NoteSO noteSO){
        if(noteSO == null || noteDataList == null) return;

        if(noteDataList.Contains(noteSO)) return;

        noteDataList.Add(noteSO);
    } 

    public bool IsNoteInDataList(NoteSO noteSO){
        if(noteSO == null || noteDataList == null) return false;
        return noteDataList.Contains(noteSO);
    }
}