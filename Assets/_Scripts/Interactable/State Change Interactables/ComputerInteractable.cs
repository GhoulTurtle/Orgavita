using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComputerInteractable : StateChangeInteractable{
    public override string InteractionPrompt => "Access";

    [Header("Computer Required References")]
    [SerializeField] private LayerMask computerScreenLayer;
    [SerializeField] private List<ComputerApplication> computerApplications;

    [Header("Computer Variables")]
    [SerializeField] private float mouseRayDistance;

    private bool isSelected = false;

    [Header("Computer Events")]
    public EventHandler OnEnterComputerState;
    public EventHandler OnExitComputerState;
    public UnityEvent<Vector2> OnCursorMove;
    public EventHandler OnCursorDown;
    public EventHandler OnCursorUp;
    
    public EventHandler<ComputerApplicationSetupEventArgs> OnSetupComputerApplications;

    public class ComputerApplicationSetupEventArgs : EventArgs{
        public List<ComputerApplication> computerApplications;

        public ComputerApplicationSetupEventArgs(List<ComputerApplication> _computerApplications){
            computerApplications = _computerApplications;
        }
    }

    public class ComputerCursorEventArgs : EventArgs{
        public Vector2 cursorPosition;

        public ComputerCursorEventArgs(Vector2 _cursorPosition){
            cursorPosition = _cursorPosition;
        }
    }

    private Camera playerCamera;

    private void Awake() {
        playerCamera = Camera.main;
    }

    private void Start() {
        OnSetupComputerApplications?.Invoke(this, new ComputerApplicationSetupEventArgs(computerApplications));
    }

    private void OnDestroy() {
        if(playerInputHandler == null) return;
        playerInputHandler.OnClickInput -= CursorInput;
    }

    private void Update() {
        if(!isSelected || isLocked) return;

        DetectMousePosition();
    }

    public override void EnterState(){
        base.EnterState();
        
        if(!isLocked){
            playerInputHandler.OnClickInput += CursorInput;
        }
        
        isSelected = true;
        OnEnterComputerState?.Invoke(this, EventArgs.Empty);
    }

    public override void ExitState(object sender, InputEventArgs e){
        base.ExitState(sender, e);

        if(!isLocked){
            playerInputHandler.OnClickInput -= CursorInput;
        }

        isSelected = false;
        OnExitComputerState?.Invoke(this, EventArgs.Empty);
    }

    public bool GetIsSelected(){
        return isSelected;
    }

    private void CursorInput(object sender, InputEventArgs e){
        switch (e.inputActionPhase){
            case UnityEngine.InputSystem.InputActionPhase.Performed: 
                OnCursorDown?.Invoke(this, EventArgs.Empty);
                break;
            case UnityEngine.InputSystem.InputActionPhase.Canceled: OnCursorUp?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    private void DetectMousePosition(){
        //Going to need to update to work with controllers 
        Ray mouseRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(mouseRay, out RaycastHit hitInfo, mouseRayDistance, computerScreenLayer, QueryTriggerInteraction.Ignore)){
            OnCursorMove?.Invoke(hitInfo.textureCoord);
        }
    }
}