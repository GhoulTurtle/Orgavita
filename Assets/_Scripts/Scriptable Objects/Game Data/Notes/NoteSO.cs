using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Note", fileName = "NewNoteSO")]
public class NoteSO : ScriptableObject{
    [Header("Note Variables")]
    public string noteTitle;
    public ItemModel noteItemModel;
    public List<string> notePages;
}