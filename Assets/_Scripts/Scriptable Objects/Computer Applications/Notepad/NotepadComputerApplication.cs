using UnityEngine;

[CreateAssetMenu(menuName = "Computer Application/Notepad Computer Application", fileName = "NewNotepadComputerApplication")]
public class NotepadComputerApplication : ComputerApplication{
    [Header("Notepad Variables")]
    public bool canType;
    public bool saveNote;
    public NoteSO noteToSave;
    [TextArea(4,4)] public string notepadContent;

    private void OnEnable() {
        if(Name == null) return;

        if(!Name.EndsWith(".txt")){
            Name += ".txt";
        }
    }
}