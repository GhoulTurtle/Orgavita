using System.Collections.Generic;
using System.Linq;
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
            for (int i = 0; i < defaultNoteDataList.Count; i++){
                noteDataList.Add(defaultNoteDataList[i]);
            }
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

    public bool IsFirstNote(){
        return noteDataList.Count == 1;
    }

    public NoteSO GetLastNoteAdded(){
        if(noteDataList == null || noteDataList.Count == 0) return null;

        return noteDataList.Last();
    }
}