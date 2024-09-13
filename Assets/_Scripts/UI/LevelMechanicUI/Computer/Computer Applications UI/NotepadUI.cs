using TMPro;
using UnityEngine;

public class NotepadUI : ApplicationUI{
    [Header("Notepad References")]
    [SerializeField] private TMP_InputField inputField;

    private NotepadComputerApplication notepadComputerApplication;

    public override void SetupApplicationUI(ComputerUI _computerUI, ComputerApplication application){
        base.SetupApplicationUI(_computerUI, application);

        notepadComputerApplication = (NotepadComputerApplication)application;

        if(notepadComputerApplication != null){
            TextContentProfile parsedTextContentProfile = TextParser.Parse(notepadComputerApplication.notepadContent);

            inputField.text = parsedTextContentProfile.textContent;

            if(!notepadComputerApplication.canType){
                inputField.interactable = false;
            }

            UIAnimator.StartTextAnimations(this, inputField.textComponent, parsedTextContentProfile);
        }
    }

    protected override void EnableApplicationUIInteractivity(){
        closeButton.interactable = true;

        if(notepadComputerApplication == null){
            Debug.Log("Its null??");
            return;
        }

        if(!notepadComputerApplication.canType){
            inputField.interactable = false;
        }
        else{
            inputField.interactable = true;
        }
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }
}