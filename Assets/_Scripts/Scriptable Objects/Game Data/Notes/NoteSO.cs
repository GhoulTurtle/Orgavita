using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Note", fileName = "NewNoteSO")]
public class NoteSO : ScriptableObject{
    [Header("Note Variables")]
    public string noteTitle;
    public Transform noteItemModel;
    public List<string> notePages;
}