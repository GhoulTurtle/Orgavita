using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComputerUI : MonoBehaviour{
    [Header("Required References")]
    
    private RectTransform canvasTransform;
    private GraphicRaycaster graphicRaycaster;

    private void Awake() {
        TryGetComponent(out canvasTransform);
        TryGetComponent(out graphicRaycaster);
    }

    public void CursorInput(Vector2 cursorCoords){
        Vector3 mousePosition = new Vector3(canvasTransform.sizeDelta.x * cursorCoords.x, canvasTransform.sizeDelta.y * cursorCoords.y, 0);
        
        PointerEventData mouseEvent = new PointerEventData(EventSystem.current);
        mouseEvent.position = mousePosition;
        
        List<RaycastResult> graphicRaycastResultsList = new List<RaycastResult>();
        graphicRaycaster.Raycast(mouseEvent, graphicRaycastResultsList);

        bool sendMouseDown = Input.GetMouseButtonDown(0);
        bool sendMouseUp = Input.GetMouseButtonUp(0);

        foreach (var raycastResult in graphicRaycastResultsList){
            if(sendMouseDown) ExecuteEvents.Execute(raycastResult.gameObject, mouseEvent, ExecuteEvents.pointerDownHandler);
            else if(sendMouseUp){
                ExecuteEvents.Execute(raycastResult.gameObject, mouseEvent, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(raycastResult.gameObject, mouseEvent, ExecuteEvents.pointerClickHandler);
            } 
        }
    }
}
