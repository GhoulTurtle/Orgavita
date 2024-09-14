using TMPro;
using UnityEngine;

public class NotepadUI : ApplicationUI{
    [Header("Notepad References")]
    [SerializeField] private TMP_InputField inputField;

    private NotepadComputerApplication notepadComputerApplication;

    private void OnDestroy() {
        StopAllCoroutines();
        computerUI.OnEnableUI -= EnableApplicationUIInteractivity;
        computerUI.OnDisableUI -= DisableApplicationUIInteractivity;
    }

    public override void SetupApplicationUI(ComputerUI _computerUI, ComputerApplication application){
        computerUI = _computerUI;
        ApplicationSO =  application;
        notepadComputerApplication = (NotepadComputerApplication)application;

        windowNameText.text = notepadComputerApplication.Name;

        computerUI.OnEnableUI += EnableApplicationUIInteractivity;
        computerUI.OnDisableUI += DisableApplicationUIInteractivity;

        EnableApplicationUIInteractivity();

        if(notepadComputerApplication != null){
            TextContentProfile parsedTextContentProfile = TextParser.Parse(notepadComputerApplication.notepadContent);

            inputField.text = parsedTextContentProfile.textContent;

            UIAnimator.StartTextAnimations(this, inputField.textComponent, parsedTextContentProfile);

            if(notepadComputerApplication.saveNote){
                PlayerNoteHandler.Instance.AttemptAddNewNoteToDataList(notepadComputerApplication.noteToSave);
                PlayerNoteHandler.Instance.AttemptShowNotePopup();
            }
        }
    }

    public override void CloseApplication(){
        DisableApplicationUIInteractivity();

        computerUI.OnEnableUI -= EnableApplicationUIInteractivity;
        computerUI.OnDisableUI -= DisableApplicationUIInteractivity;
        
        computerUI.CloseApplication();
    }

    public override void DisableApplicationUIInteractivity(){
        base.DisableApplicationUIInteractivity();
    }

    public override void EnableApplicationUIInteractivity(){
        base.EnableApplicationUIInteractivity();

        if(notepadComputerApplication == null) return;

        if(!notepadComputerApplication.canType){
            inputField.interactable = false;
        }
        else{
            inputField.interactable = true;
        }
    }
}