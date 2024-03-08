using System;
using UnityEngine;
using UnityEngine.Events;

public class ComputerInteractable : StateChangeInteractable{
    public override string InteractionPrompt => "Access";

    [Header("Computer Required References")]
    [SerializeField] private LayerMask computerScreenLayer;
    [SerializeField] private float mouseRayDistance;

    private bool isSelected = false;

    [Header("Computer Events")]
    public UnityEvent<Vector2> OnCursorMove;
    public EventHandler OnCursorDown;
    public EventHandler OnCursorUp;

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

    private void OnDestroy() {
        if(playerInputHandler == null) return;
        playerInputHandler.OnClickInput -= CursorInput;
    }

    private void Update() {
        if(!isSelected) return;

        DetectMousePosition();
    }

    public override void EnterState(){
        base.EnterState();
        playerInputHandler.OnClickInput += CursorInput;
        isSelected = true;
    }

    public override void ExitState(object sender, PlayerInputHandler.InputEventArgs e){
        base.ExitState(sender, e);
        playerInputHandler.OnClickInput -= CursorInput;
        isSelected = false;
    }

    private void CursorInput(object sender, PlayerInputHandler.InputEventArgs e){
        switch (e.inputActionPhase){
            case UnityEngine.InputSystem.InputActionPhase.Performed: OnCursorDown?.Invoke(this, EventArgs.Empty);
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