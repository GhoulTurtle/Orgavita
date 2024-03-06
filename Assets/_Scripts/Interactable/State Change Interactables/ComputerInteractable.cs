using System;
using UnityEngine;
using UnityEngine.Events;

public class ComputerInteractable : StateChangeInteractable{
    public override string InteractionPrompt => "Access";

    [Header("Computer Required References")]
    [SerializeField] private LayerMask computerScreenLayer;
    [SerializeField] private float mouseRayDistance;

    [Header("Computer Events")]
    [SerializeField] private UnityEvent<Vector2> OnCursorInput = new UnityEvent<Vector2>();

    private bool isSelected = false;

    private Camera playerCamera;

    private void Awake() {
        playerCamera = Camera.main;
    }

    public override void EnterState(){
        base.EnterState();
        isSelected = true;
    }

    public override void ExitState(object sender, EventArgs e){
        base.ExitState(sender, e);
        isSelected = false;
    }

    private void Update() {
        if(!isSelected) return;
        DetectMousePosition();
    }

    private void DetectMousePosition(){
        Ray mouseRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(mouseRay, out RaycastHit hitInfo, mouseRayDistance, computerScreenLayer, QueryTriggerInteraction.Ignore)){
            OnCursorInput?.Invoke(hitInfo.textureCoord);
        }
    }
}
